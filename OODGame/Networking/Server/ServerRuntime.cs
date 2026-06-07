using OODGame.Networking.Protocol;
using OODGame.Networking.Transport;
using OODGame.Players;
using OODGame.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OODGame.Networking.Server
{
    public sealed class ServerRuntime
    {
        private readonly StartupOptions _options;
        private readonly Game _game;
        private readonly TcpListener _listener;
        private readonly Dictionary<int, ClientConnection> _connections = new Dictionary<int, ClientConnection>();
        private readonly object _connectionsLock = new object();

        public ServerRuntime(StartupOptions options, GameConfig config)
        {
            _options = options;
            _game = new Game(config, createLocalPlayer: false);
            _game.IsRunning = true;
            _listener = new TcpListener(IPAddress.Any, _options.Port);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _listener.Start();
            Console.WriteLine($"Server listening on 0.0.0.0:{_options.Port}");

            try
            {
                Task acceptLoop = AcceptLoopAsync(cancellationToken);
                Task processLoop = ProcessActionsLoopAsync(cancellationToken);
                await Task.WhenAll(acceptLoop, processLoop).ConfigureAwait(false);
            }
            finally
            {
                _listener.Stop();
            }
        }

        private async Task AcceptLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                _ = Task.Run(() => InitializeClientAsync(tcpClient, cancellationToken), cancellationToken);
            }
        }

        private async Task InitializeClientAsync(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            var channel = new JsonLineChannel(tcpClient);

            int playerId = AllocatePlayerId();
            if (playerId < 1)
            {
                await channel.SendAsync(
                    ProtocolJson.CreateEnvelope(
                        ProtocolMessageType.Error,
                        null,
                        new ErrorPayload { Message = "Server is full (max 9 players)." }),
                    cancellationToken).ConfigureAwait(false);
                await channel.DisposeAsync().ConfigureAwait(false);
                return;
            }

            var player = CreatePlayer(playerId);
            if (!_game.TryAddPlayer(playerId, player))
            {
                await channel.SendAsync(
                    ProtocolJson.CreateEnvelope(
                        ProtocolMessageType.Error,
                        null,
                        new ErrorPayload { Message = "Failed to register player." }),
                    cancellationToken).ConfigureAwait(false);
                await channel.DisposeAsync().ConfigureAwait(false);
                return;
            }

            var connection = new ClientConnection(playerId, tcpClient, channel);
            RegisterConnection(connection);
            IPEndPoint? remote = tcpClient.Client.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Player {playerId} connected from {remote?.Address}:{remote?.Port}. Connected players: {GetConnectionCount()}.");

            await SendInitialStateAsync(connection, cancellationToken).ConfigureAwait(false);
            await BroadcastStateAsync(StateUpdateType.PlayerJoined, $"Player {playerId} joined.", cancellationToken).ConfigureAwait(false);

            _ = Task.Run(() => ReceiveClientActionsAsync(connection, cancellationToken), cancellationToken);
        }

        private async Task SendInitialStateAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            var payload = new InitialStatePayload
            {
                AssignedPlayerId = connection.PlayerId,
                State = _game.CreateCurrentState()
            };

            await connection.Channel.SendAsync(
                ProtocolJson.CreateEnvelope(ProtocolMessageType.InitialState, connection.PlayerId, payload),
                cancellationToken).ConfigureAwait(false);
        }

        private async Task ReceiveClientActionsAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    MessageEnvelope? envelope = await connection.Channel.ReceiveAsync(cancellationToken).ConfigureAwait(false);
                    if (envelope is null)
                        break;

                    if (envelope.Type != ProtocolMessageType.PlayerAction)
                        continue;

                    if (!ProtocolJson.TryGetPayload<PlayerActionPayload>(envelope, out var actionPayload) || actionPayload is null)
                        continue;

                    _game.EnqueueIncomingAction(connection.PlayerId, actionPayload.Action, actionPayload.ItemIndex, actionPayload.PreferredHand);
                }
            }
            catch
            {
            }
            finally
            {
                await DisconnectClientAsync(connection.PlayerId, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task ProcessActionsLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                List<Game.ProcessedPlayerAction> processed = _game.ProcessPendingActions();
                foreach (Game.ProcessedPlayerAction action in processed)
                {
                    if (action.Applied)
                    {
                        StateUpdateType updateType = GetAppliedUpdateType(action.Action);
                        string description = $"P{action.PlayerId}: {action.Action} (idx={action.ItemIndex?.ToString() ?? "-"}, hand={action.PreferredHand ?? "-"})";
                        await BroadcastStateAsync(updateType, description, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        string reason = $"Rejected P{action.PlayerId}:{action.Action} (idx={action.ItemIndex?.ToString() ?? "-"}, hand={action.PreferredHand ?? "-"}).";
                        await BroadcastStateAsync(StateUpdateType.PlayerActionRejected, reason, cancellationToken).ConfigureAwait(false);
                    }
                }

                await Task.Delay(30, cancellationToken).ConfigureAwait(false);
            }
        }

        private static StateUpdateType GetAppliedUpdateType(PlayerActionType action)
        {
            return action switch
            {
                PlayerActionType.MoveUp or PlayerActionType.MoveDown or PlayerActionType.MoveLeft or PlayerActionType.MoveRight => StateUpdateType.PlayerMoved,
                PlayerActionType.Attack => StateUpdateType.Attack,
                PlayerActionType.Interact => StateUpdateType.FullState,
                PlayerActionType.PickupItem => StateUpdateType.SoundEvent,
                PlayerActionType.EquipItem => StateUpdateType.FullState,
                PlayerActionType.DropItem => StateUpdateType.FullState,
                PlayerActionType.Quit => StateUpdateType.PlayerLeft,
                _ => StateUpdateType.FullState
            };
        }

        private async Task BroadcastStateAsync(StateUpdateType updateType, string description, CancellationToken cancellationToken)
        {
            var payload = new StateUpdatePayload
            {
                UpdateType = updateType,
                Description = description,
                State = _game.CreateCurrentState()
            };

            MessageEnvelope envelope = ProtocolJson.CreateEnvelope(ProtocolMessageType.StateUpdate, null, payload);

            List<ClientConnection> snapshot;
            lock (_connectionsLock)
            {
                snapshot = _connections.Values.ToList();
            }

            foreach (ClientConnection connection in snapshot)
            {
                try
                {
                    await connection.Channel.SendAsync(envelope, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                }
            }
        }

        private async Task DisconnectClientAsync(int playerId, CancellationToken cancellationToken)
        {
            ClientConnection? removed = null;
            lock (_connectionsLock)
            {
                if (_connections.TryGetValue(playerId, out removed))
                {
                    _connections.Remove(playerId);
                }
            }

            if (removed is null)
                return;

            await removed.Channel.DisposeAsync().ConfigureAwait(false);
            _game.RemovePlayer(playerId);
            Console.WriteLine($"Player {playerId} disconnected. Connected players: {GetConnectionCount()}.");
            await BroadcastStateAsync(StateUpdateType.PlayerLeft, $"Player {playerId} left.", cancellationToken).ConfigureAwait(false);
        }

        private int AllocatePlayerId()
        {
            lock (_connectionsLock)
            {
                for (int id = 1; id <= Game.MaxPlayers; id++)
                {
                    if (!_connections.ContainsKey(id))
                        return id;
                }
            }

            return -1;
        }

        private void RegisterConnection(ClientConnection connection)
        {
            lock (_connectionsLock)
            {
                _connections[connection.PlayerId] = connection;
            }
        }

        private int GetConnectionCount()
        {
            lock (_connectionsLock)
            {
                return _connections.Count;
            }
        }

        private Player CreatePlayer(int playerId)
        {
            (int x, int y) = GetSpawnPosition(playerId);
            return new Player(x, y, $"Player{playerId}");
        }

        private (int x, int y) GetSpawnPosition(int playerId)
        {
            (int x, int y)[] positions =
            {
                (Game.RoomWidth / 2, Game.RoomHeight / 2),
                (2, 2),
                (Game.RoomWidth - 3, 2),
                (2, Game.RoomHeight - 3),
                (Game.RoomWidth - 3, Game.RoomHeight - 3),
                (Game.RoomWidth / 2, 2),
                (Game.RoomWidth / 2, Game.RoomHeight - 3),
                (2, Game.RoomHeight / 2),
                (Game.RoomWidth - 3, Game.RoomHeight / 2)
            };

            foreach (var position in positions)
            {
                if (IsSpawnCandidateValid(position.x, position.y))
                    return position;
            }

            for (int y = 1; y < Game.RoomHeight - 1; y++)
            {
                for (int x = 1; x < Game.RoomWidth - 1; x++)
                {
                    if (IsSpawnCandidateValid(x, y))
                        return (x, y);
                }
            }

            return (Game.RoomWidth / 2, Game.RoomHeight / 2);
        }

        private bool IsSpawnCandidateValid(int x, int y)
        {
            if (!_game.CurrentRoom.Grid[y, x].CanEnter())
                return false;

            foreach (Player other in _game.Players.Values)
            {
                if (other.Xpos == x && other.Ypos == y)
                    return false;
            }

            return true;
        }

        private sealed class ClientConnection
        {
            public int PlayerId { get; }
            public TcpClient TcpClient { get; }
            public JsonLineChannel Channel { get; }

            public ClientConnection(int playerId, TcpClient tcpClient, JsonLineChannel channel)
            {
                PlayerId = playerId;
                TcpClient = tcpClient;
                Channel = channel;
            }
        }
    }
}
