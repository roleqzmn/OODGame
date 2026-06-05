using OODGame.Items;
using OODGame.Dungeon;
using OODGame.Entities;
using OODGame.Items.Unequipable;
using OODGame.Items.Weapons;
using OODGame.Map;
using OODGame.Players;
using OODGame.Actions;
using OODGame.Input;
using OODGame.Logger;
using OODGame.Networking.Protocol;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace OODGame
{
    public class Game
    {
        public Room CurrentRoom { get; set; }
        public Room[,] Map { get; set; }
        private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();
        private readonly object _stateLock = new object();
        private readonly ConcurrentQueue<QueuedPlayerAction> _pendingActions = new ConcurrentQueue<QueuedPlayerAction>();
        private long _nextActionOrder;
        public IReadOnlyDictionary<int, Player> Players => _players;
        public int LocalPlayerId { get; private set; } = 1;
        public Player Player => _players[LocalPlayerId];
        public int CurrentMapX { get; set; }
        public int CurrentMapY { get; set; }
        public bool IsRunning { get; set; }
        
        public const int RoomWidth = 40;
        public const int RoomHeight = 20;
        public const int MaxPlayers = 9;
        private readonly IDungeonTheme _theme;
        private readonly IInputSource _inputSource;
        private readonly Random _roomRandom = new Random();
        private Actions.Actions actions { get; set; }
        private string LogFile;
        public Game(GameConfig config, IInputSource? inputSource = null)
        {
            _inputSource = inputSource ?? new ConsoleInputSource();
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string logDir = Path.GetDirectoryName(config.LogPath) ?? ".";
            LogFile = Path.Combine(logDir, $"{config.PlayerName}_{timestamp}.txt");
            EventLogger.GetInstance(LogFile);
            _theme = DungeonThemeFactory.Create(config.DungeonTheme);

            Map = new Room[3, 3];
            CurrentMapX = 1;
            CurrentMapY = 1;

            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    Map[y, x] = GenerateConnectedRoom(x, y, isCenter: x == 1 && y == 1);

            CurrentRoom = Map[CurrentMapY, CurrentMapX];
            var localPlayer = new Player(RoomWidth / 2, RoomHeight / 2, config.PlayerName);
            localPlayer.EventBus = CurrentRoom.EventBus;
            _players[LocalPlayerId] = localPlayer;
            actions = new Actions.Actions(this, _inputSource);
        }

        private Room GenerateConnectedRoom(int mapX, int mapY, bool isCenter)
        {
            Room room = _theme.GenerationStrategy.Generate(
                new DungeonBuilder(RoomWidth, RoomHeight),
                mapX, mapY,
                _theme.EnemyGroups,
                _theme.GetPossibleItems(),
                _theme.CreateArtifact(),
                placeArtifact: isCenter);

            RegisterRoomEnemySubscriptions(room);
            return room;
        }

        private static void RegisterRoomEnemySubscriptions(Room room)
        {
            for (int y = 0; y < room.Height; y++)
            {
                for (int x = 0; x < room.Width; x++)
                {
                    if (room.Grid[y, x] is EmptyTile emptyTile && emptyTile.HasEnemy)
                    {
                        emptyTile.Enemy!.SetSpatialContext(x, y, room.Navigator);
                        room.EventBus.Subscribe(emptyTile.Enemy);
                    }
                }
            }
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Draw.DrawIntro(_theme);

            Console.Clear();
            Draw.DrawRoom(this);
            Draw.DrawPlayers(this);
            Draw.DrawUI(this);
            Draw.DrawEq(Player);
            Draw.DrawRecentLogs();

            IsRunning = true;
            while (IsRunning)
            {
                HandleInput();
            }
        }
        private void HandleInput()
        {
            var key = _inputSource.ReadKey();
            actions.Handle(key);
        }

        public void RedrawScreen()
        {
            Console.Clear();
            Draw.DrawRoom(this);
            Draw.DrawPlayers(this);
            Draw.DrawUI(this);
            Draw.DrawEq(Player);
            Draw.DrawRecentLogs();
        }

        public void RefreshUI()
        {
            Draw.DrawUI(this);
            Draw.DrawEq(Player);
            Draw.DrawRecentLogs();
        }

      
        public void DropItem(Item item, int x, int y)
        {
            var itemTile = CurrentRoom.Grid[y, x];
            if (itemTile != null)
            {
                itemTile.PlaceItem(item);
            }
            else
            {
                itemTile = new EmptyTile();
                itemTile.PlaceItem(item);
            }
        }

        public bool TryGetPlayer(int playerId, out Player player)
        {
            lock (_stateLock)
            {
                return _players.TryGetValue(playerId, out player!);
            }
        }

        public bool TryAddPlayer(int playerId, Player player)
        {
            lock (_stateLock)
            {
                if (playerId < 1 || playerId > MaxPlayers)
                    return false;
                if (_players.ContainsKey(playerId))
                    return false;

                player.EventBus = CurrentRoom.EventBus;
                _players[playerId] = player;
                return true;
            }
        }

        public bool RemovePlayer(int playerId)
        {
            lock (_stateLock)
            {
                if (playerId == LocalPlayerId)
                    return false;
                return _players.Remove(playerId);
            }
        }

        public bool ApplyAction(int playerId, PlayerActionType action)
        {
            lock (_stateLock)
            {
                return actions.ApplyPlayerAction(playerId, action);
            }
        }

        public long EnqueueIncomingAction(int playerId, PlayerActionType action)
        {
            long order = Interlocked.Increment(ref _nextActionOrder);
            _pendingActions.Enqueue(new QueuedPlayerAction(order, playerId, action));
            return order;
        }

        public List<ProcessedPlayerAction> ProcessPendingActions()
        {
            var processed = new List<ProcessedPlayerAction>();

            while (_pendingActions.TryDequeue(out QueuedPlayerAction queued))
            {
                bool applied;
                lock (_stateLock)
                {
                    applied = actions.ApplyPlayerAction(queued.PlayerId, queued.Action);
                }

                processed.Add(new ProcessedPlayerAction(queued.Order, queued.PlayerId, queued.Action, applied));
            }

            processed.Sort(static (left, right) => left.Order.CompareTo(right.Order));
            return processed;
        }

        public GameStateDto CreateCurrentState()
        {
            lock (_stateLock)
            {
                var roomRows = new List<string>(CurrentRoom.Height);
                for (int y = 0; y < CurrentRoom.Height; y++)
                {
                    char[] row = new char[CurrentRoom.Width];
                    for (int x = 0; x < CurrentRoom.Width; x++)
                    {
                        row[x] = CurrentRoom.Grid[y, x].Symbol;
                    }
                    roomRows.Add(new string(row));
                }

                List<PlayerStateDto> players = _players
                    .Select(entry => new PlayerStateDto
                    {
                        PlayerId = entry.Key,
                        Name = entry.Value.Name,
                        X = entry.Value.Xpos,
                        Y = entry.Value.Ypos,
                        Health = entry.Value.Stats.Health,
                        MaxHealth = entry.Value.Stats.MaxHealth
                    })
                    .OrderBy(player => player.PlayerId)
                    .ToList();

                return new GameStateDto
                {
                    CurrentMapX = CurrentMapX,
                    CurrentMapY = CurrentMapY,
                    CurrentRoomRows = roomRows,
                    Players = players
                };
            }
        }

        public readonly record struct QueuedPlayerAction(long Order, int PlayerId, PlayerActionType Action);
        public readonly record struct ProcessedPlayerAction(long Order, int PlayerId, PlayerActionType Action, bool Applied);
        
    }
}
