using OODGame.Networking.Protocol;
using OODGame.Networking.Protocol;
using OODGame.Networking.Transport;
using OODGame.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OODGame.Networking.Client
{
    public sealed class ClientRuntime
    {
        private readonly StartupOptions _options;
        private readonly object _stateLock = new object();
        private GameStateDto? _state;
        private int _localPlayerId;
        private int _selectedItemIndex;
        private int _fightSelectedIndex;
        private List<string>? _lastRenderedRows;
        private bool _inventoryOpen;
        private bool _pickupArmed;
        private int _pickupArmedIndex;
        private string _preferredEquipHand = "right";
        private bool _wasInCombatLastFrame;

        public ClientRuntime(StartupOptions options)
        {
            _options = options;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            using var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(_options.Ip, _options.Port, cancellationToken).ConfigureAwait(false);
            await using var channel = new JsonLineChannel(tcpClient);

            MessageEnvelope? initialEnvelope = await channel.ReceiveAsync(cancellationToken).ConfigureAwait(false);
            if (initialEnvelope is null || initialEnvelope.Type != ProtocolMessageType.InitialState)
                throw new InvalidOperationException("Did not receive initial game state from server.");

            if (!ProtocolJson.TryGetPayload<InitialStatePayload>(initialEnvelope, out var initialPayload) || initialPayload is null)
                throw new InvalidOperationException("Initial game state payload is invalid.");

            _localPlayerId = initialPayload.AssignedPlayerId;
            lock (_stateLock)
            {
                _state = initialPayload.State;
            }

            _inventoryOpen = false;
            _lastRenderedRows = null;
            _pickupArmed = false;
            _pickupArmedIndex = 0;
            _fightSelectedIndex = 0;
            _wasInCombatLastFrame = false;

            RenderState("Connected.");

            Task receiveTask = ReceiveLoopAsync(channel, cancellationToken);
            Task inputTask = InputLoopAsync(channel, cancellationToken);
            await Task.WhenAny(receiveTask, inputTask).ConfigureAwait(false);
        }

        private async Task ReceiveLoopAsync(JsonLineChannel channel, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                MessageEnvelope? envelope = await channel.ReceiveAsync(cancellationToken).ConfigureAwait(false);
                if (envelope is null)
                    break;

                switch (envelope.Type)
                {
                    case ProtocolMessageType.StateUpdate:
                        if (ProtocolJson.TryGetPayload<StateUpdatePayload>(envelope, out var update) && update?.State is not null)
                        {
                            lock (_stateLock)
                            {
                                _state = update.State;
                            }

                            RenderState(update.Description ?? "State updated.");
                        }
                        else if (ProtocolJson.TryGetPayload<StateUpdatePayload>(envelope, out update) && update is not null)
                        {
                            RenderState(update.Description ?? "State update received.");
                        }
                        break;
                    case ProtocolMessageType.Error:
                        if (ProtocolJson.TryGetPayload<ErrorPayload>(envelope, out var error) && error is not null)
                            RenderState($"Server error: {error.Message}");
                        break;
                }
            }
        }

        private async Task InputLoopAsync(JsonLineChannel channel, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ConsoleKey key = Console.ReadKey(intercept: true).Key;

                if (TryGetMyState(out PlayerStateDto? me) && me is not null && me.IsInCombat)
                {
                    if (key == ConsoleKey.UpArrow)
                    {
                        _fightSelectedIndex = Math.Max(0, _fightSelectedIndex - 1);
                        RenderState($"Selected fight action: {_fightSelectedIndex + 1}");
                        continue;
                    }

                    if (key == ConsoleKey.DownArrow)
                    {
                        int maxIdx = me.Combat?.ActionNames.Count > 0 ? me.Combat.ActionNames.Count - 1 : 2;
                        _fightSelectedIndex = Math.Min(maxIdx, _fightSelectedIndex + 1);
                        RenderState($"Selected fight action: {_fightSelectedIndex + 1}");
                        continue;
                    }

                    if (key == ConsoleKey.E)
                    {
                        await SendActionAsync(channel, PlayerActionType.Attack, _fightSelectedIndex, cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    if (key == ConsoleKey.Escape)
                    {
                        await SendActionAsync(channel, PlayerActionType.Quit, 0, cancellationToken).ConfigureAwait(false);
                        break;
                    }

                    RenderState("In combat: use Up/Down and E.");
                    continue;
                }

                if (TryReadItemIndexHotkey(key, out int selectedIndex))
                {
                    _selectedItemIndex = selectedIndex;
                    _pickupArmed = false;
                    RenderState($"Selected item index: {_selectedItemIndex}");
                    continue;
                }

                if (key == ConsoleKey.I)
                {
                    _inventoryOpen = !_inventoryOpen;
                    _pickupArmed = false;
                    RenderState(_inventoryOpen ? "Inventory opened." : "Inventory closed.");
                    continue;
                }

                if (_inventoryOpen && key == ConsoleKey.Escape)
                {
                    _inventoryOpen = false;
                    _pickupArmed = false;
                    RenderState("Inventory closed.");
                    continue;
                }

                if (!_inventoryOpen && key == ConsoleKey.E)
                {
                    if (!_pickupArmed)
                    {
                        if (TryGetPickupPreview(out int boundedIndex, out InventoryItemDto? item))
                        {
                            _pickupArmed = true;
                            _pickupArmedIndex = boundedIndex;
                            RenderState($"Ready to pick: {item!.Symbol}-{item.Name} (idx={boundedIndex}). Press E again to confirm.");
                        }
                        else
                        {
                            RenderState("No item to pick on your tile.");
                        }

                        continue;
                    }

                    await SendActionAsync(channel, PlayerActionType.PickupItem, _pickupArmedIndex, cancellationToken).ConfigureAwait(false);
                    _pickupArmed = false;
                    continue;
                }

                if (_inventoryOpen)
                {
                    if (key == ConsoleKey.E)
                    {
                        RenderState("Inventory mode: use R to equip selected item.");
                        continue;
                    }

                    if (key == ConsoleKey.LeftArrow)
                    {
                        _selectedItemIndex = Math.Max(0, _selectedItemIndex - 1);
                        RenderState($"Selected item index: {_selectedItemIndex}");
                        continue;
                    }

                    if (key == ConsoleKey.RightArrow)
                    {
                        _selectedItemIndex++;
                        _pickupArmed = false;
                        RenderState($"Selected item index: {_selectedItemIndex}");
                        continue;
                    }

                    if (key == ConsoleKey.R)
                    {
                        await SendActionAsync(channel, PlayerActionType.EquipItem, _selectedItemIndex, cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    if (key == ConsoleKey.L)
                    {
                        _preferredEquipHand = "left";
                        RenderState("Preferred equip hand: LEFT.");
                        continue;
                    }

                    if (key == ConsoleKey.K)
                    {
                        _preferredEquipHand = "right";
                        RenderState("Preferred equip hand: RIGHT.");
                        continue;
                    }
                }

                if (!TryMapKey(key, out PlayerActionType action))
                    continue;

                _pickupArmed = false;
                await SendActionAsync(channel, action, _selectedItemIndex, cancellationToken).ConfigureAwait(false);

                if (action == PlayerActionType.Quit)
                    break;
            }
        }

        private void RenderState(string status)
        {
            GameStateDto? state;
            lock (_stateLock)
            {
                state = _state;
            }

            if (state is null)
                return;

            bool inCombatNow = state.Players.Any(player => player.PlayerId == _localPlayerId && player.IsInCombat);
            if (inCombatNow != _wasInCombatLastFrame)
            {
                _lastRenderedRows = null;
                Console.Clear();
            }
            _wasInCombatLastFrame = inCombatNow;

            PlayerStateDto? me = state.Players.FirstOrDefault(player => player.PlayerId == _localPlayerId);
            if (me is not null)
            {
                if (me.IsInCombat && me.Combat is not null)
                {
                    Console.Clear();
                    _inventoryOpen = false;
                    _pickupArmed = false;
                    _fightSelectedIndex = Math.Clamp(_fightSelectedIndex, 0, Math.Max(0, me.Combat.ActionNames.Count - 1));
                    global::OODGame.Draw.DrawNetworkFight(me, me.Combat, _fightSelectedIndex);
                    global::OODGame.Draw.ClearNetworkInventoryPanel();
                }
                else
                {
                    List<string> currentRows = BuildRenderedRows(state);
                    if (_lastRenderedRows is null)
                    {
                        Console.Clear();
                        global::OODGame.Draw.DrawNetworkRoom(currentRows);
                    }
                    else
                    {
                        global::OODGame.Draw.RedrawNetworkChangedCells(_lastRenderedRows, currentRows);
                    }

                    _lastRenderedRows = currentRows;

                    global::OODGame.Draw.DrawNetworkUI(me);
                    global::OODGame.Draw.DrawNetworkEq(me);

                    if (_inventoryOpen)
                    {
                        if (me.InventoryItems.Count > 0)
                            _selectedItemIndex = Math.Clamp(_selectedItemIndex, 0, me.InventoryItems.Count - 1);
                        else
                            _selectedItemIndex = 0;

                        global::OODGame.Draw.DrawNetworkInventory(me, _selectedItemIndex, _preferredEquipHand);
                    }
                    else
                    {
                        ItemTileStateDto? tile = TryGetItemTileAtPlayer(state, me);
                        if (tile is not null)
                        {
                            int bounded = Math.Clamp(_selectedItemIndex, 0, tile.Items.Count - 1);
                            global::OODGame.Draw.DrawNetworkPickupItems(tile, bounded, _pickupArmed);
                        }
                        else
                        {
                            global::OODGame.Draw.ClearNetworkInventoryPanel();
                        }
                    }
                }
            }
            else
            {
                global::OODGame.Draw.ClearNetworkInventoryPanel();
            }
            int infoY = state.CurrentRoomRows.Count + 1;
            Console.SetCursorPosition(0, infoY);
            Console.Write(new string(' ', Math.Max(1, Console.WindowWidth - 1)));
            Console.SetCursorPosition(0, infoY + 1);
            Console.Write(new string(' ', Math.Max(1, Console.WindowWidth - 1)));
            Console.SetCursorPosition(0, infoY + 2);
            Console.Write(new string(' ', Math.Max(1, Console.WindowWidth - 1)));
            Console.SetCursorPosition(0, infoY + 3);
            Console.Write(new string(' ', Math.Max(1, Console.WindowWidth - 1)));

            Console.SetCursorPosition(0, infoY);
            Console.Write($"You are player {_localPlayerId}. Controls: W/A/S/D, E=pickup(confirm), F=attack, Q=drop, I=inventory, ESC=quit.");
            Console.SetCursorPosition(0, infoY + 1);
            Console.Write($"Select item index with keys 0-9 (current: {_selectedItemIndex}), hand in inventory: L=left, K=right ({_preferredEquipHand}), ESC closes inventory.");
            if (me is not null)
            {
                Console.SetCursorPosition(0, infoY + 2);
                if (me.IsInCombat && me.Combat is not null)
                    Console.Write($"Player {_localPlayerId}: IN COMBAT with {me.Combat.EnemyName} ({me.Combat.EnemyHealth}/{me.Combat.EnemyMaxHealth})");
                else
                    Console.Write($"Player {_localPlayerId}: pos=({me.X},{me.Y}) HP={me.Health}/{me.MaxHealth} Inventory={me.InventoryCount}");
            }
            else
            {
                Console.SetCursorPosition(0, infoY + 2);
                Console.Write($"Player {_localPlayerId}: not found in snapshot.");
            }
            Console.SetCursorPosition(0, infoY + 3);
            Console.Write(status);
        }

        private async Task SendActionAsync(JsonLineChannel channel, PlayerActionType action, int itemIndex, CancellationToken cancellationToken)
        {
            var payload = new PlayerActionPayload { Action = action, ItemIndex = itemIndex, PreferredHand = _preferredEquipHand };
            MessageEnvelope envelope = ProtocolJson.CreateEnvelope(ProtocolMessageType.PlayerAction, _localPlayerId, payload);
            await channel.SendAsync(envelope, cancellationToken).ConfigureAwait(false);
        }

        private bool TryGetPickupPreview(out int boundedIndex, out InventoryItemDto? item)
        {
            boundedIndex = 0;
            item = null;

            GameStateDto? state;
            lock (_stateLock)
            {
                state = _state;
            }

            if (state is null)
                return false;

            PlayerStateDto? me = state.Players.FirstOrDefault(player => player.PlayerId == _localPlayerId);
            if (me is null)
                return false;

            ItemTileStateDto? tile = TryGetItemTileAtPlayer(state, me);
            if (tile is null)
                return false;

            boundedIndex = Math.Clamp(_selectedItemIndex, 0, tile.Items.Count - 1);
            item = tile.Items[boundedIndex];
            return true;
        }

        private static ItemTileStateDto? TryGetItemTileAtPlayer(GameStateDto state, PlayerStateDto me)
        {
            return state.ItemTiles.FirstOrDefault(it => it.X == me.X && it.Y == me.Y && it.Items.Count > 0);
        }

        private bool TryGetMyState(out PlayerStateDto? player)
        {
            player = null;
            GameStateDto? state;
            lock (_stateLock)
            {
                state = _state;
            }

            if (state is null)
                return false;

            player = state.Players.FirstOrDefault(p => p.PlayerId == _localPlayerId);
            return player is not null;
        }

        private static List<string> BuildRenderedRows(GameStateDto state)
        {
            var rows = state.CurrentRoomRows
                .Select(row => row.ToCharArray())
                .ToList();

            foreach (PlayerStateDto player in state.Players)
            {
                if (player.PlayerId is < 1 or > 9)
                    continue;
                if (player.Y < 0 || player.Y >= rows.Count)
                    continue;
                if (player.X < 0 || player.X >= rows[player.Y].Length)
                    continue;

                rows[player.Y][player.X] = (char)('0' + player.PlayerId);
            }

            return rows.Select(chars => new string(chars)).ToList();
        }

        private static bool TryReadItemIndexHotkey(ConsoleKey key, out int itemIndex)
        {
            switch (key)
            {
                case ConsoleKey.D0:
                case ConsoleKey.NumPad0:
                    itemIndex = 0;
                    return true;
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    itemIndex = 1;
                    return true;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    itemIndex = 2;
                    return true;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    itemIndex = 3;
                    return true;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    itemIndex = 4;
                    return true;
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    itemIndex = 5;
                    return true;
                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    itemIndex = 6;
                    return true;
                case ConsoleKey.D7:
                case ConsoleKey.NumPad7:
                    itemIndex = 7;
                    return true;
                case ConsoleKey.D8:
                case ConsoleKey.NumPad8:
                    itemIndex = 8;
                    return true;
                case ConsoleKey.D9:
                case ConsoleKey.NumPad9:
                    itemIndex = 9;
                    return true;
                default:
                    itemIndex = 0;
                    return false;
            }
        }

        private static bool TryMapKey(ConsoleKey key, out PlayerActionType action)
        {
            switch (key)
            {
                case ConsoleKey.W:
                    action = PlayerActionType.MoveUp;
                    return true;
                case ConsoleKey.S:
                    action = PlayerActionType.MoveDown;
                    return true;
                case ConsoleKey.A:
                    action = PlayerActionType.MoveLeft;
                    return true;
                case ConsoleKey.D:
                    action = PlayerActionType.MoveRight;
                    return true;
                case ConsoleKey.E:
                    action = PlayerActionType.PickupItem;
                    return true;
                case ConsoleKey.F:
                    action = PlayerActionType.Attack;
                    return true;
                case ConsoleKey.R:
                    action = PlayerActionType.EquipItem;
                    return true;
                case ConsoleKey.Q:
                    action = PlayerActionType.DropItem;
                    return true;
                case ConsoleKey.Escape:
                    action = PlayerActionType.Quit;
                    return true;
                default:
                    action = default;
                    return false;
            }
        }
    }
}
