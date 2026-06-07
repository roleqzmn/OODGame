using OODGame.Items;
using OODGame.Dungeon;
using OODGame.Entities;
using OODGame.Items.Unequipable;
using OODGame.Items.Weapons;
using OODGame.Map;
using OODGame.Players;
using OODGame.Actions;
using OODGame.Fight;
using OODGame.Fight.Actions;
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
        private readonly Dictionary<int, CombatSession> _combatSessions = new Dictionary<int, CombatSession>();
        private readonly object _stateLock = new object();
        private readonly ConcurrentQueue<QueuedPlayerAction> _pendingActions = new ConcurrentQueue<QueuedPlayerAction>();
        private long _nextActionOrder;
        private readonly bool _hasLocalPlayer;
        public IReadOnlyDictionary<int, Player> Players => _players;
        public int LocalPlayerId { get; private set; } = 1;
        public Player Player => _players[LocalPlayerId];
        public bool HasLocalPlayer => _hasLocalPlayer;
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
        public Game(GameConfig config, IInputSource? inputSource = null, bool createLocalPlayer = true)
        {
            _hasLocalPlayer = createLocalPlayer;
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

            if (createLocalPlayer)
            {
                var localPlayer = new Player(RoomWidth / 2, RoomHeight / 2, config.PlayerName);
                localPlayer.EventBus = CurrentRoom.EventBus;
                _players[LocalPlayerId] = localPlayer;
            }

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
            if (!HasLocalPlayer)
                throw new InvalidOperationException("Local game mode requires a local player.");

            Console.CursorVisible = false;
            Draw.DrawIntro(_theme);

            Console.Clear();
            Draw.DrawRoom(this);
            Draw.DrawPlayers(this);
            if (HasLocalPlayer)
            {
                Draw.DrawUI(this);
                Draw.DrawEq(Player);
            }
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
            if (HasLocalPlayer)
            {
                Draw.DrawUI(this);
                Draw.DrawEq(Player);
            }
            Draw.DrawRecentLogs();
        }

        public void RefreshUI()
        {
            if (HasLocalPlayer)
            {
                Draw.DrawUI(this);
                Draw.DrawEq(Player);
            }
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
                if (_hasLocalPlayer && playerId == LocalPlayerId)
                    return false;
                _combatSessions.Remove(playerId);
                return _players.Remove(playerId);
            }
        }

        public bool TryStartCombat(int playerId, EmptyTile tile)
        {
            if (!_players.TryGetValue(playerId, out Player? player))
                return false;
            if (!tile.HasEnemy || tile.Enemy is null)
                return false;

            if (_combatSessions.ContainsKey(playerId))
                return true;

            var context = new FightContext(player, tile.Enemy)
            {
                LastLog = $"Encounter started with {tile.Enemy.Name}."
            };

            _combatSessions[playerId] = new CombatSession(tile, context);
            return true;
        }

        public bool IsPlayerInCombat(int playerId)
        {
            return _combatSessions.ContainsKey(playerId);
        }

        public bool HasActiveCombats()
        {
            return _combatSessions.Count > 0;
        }

        public bool TryGetCombatSession(int playerId, out CombatSession session)
        {
            return _combatSessions.TryGetValue(playerId, out session!);
        }

        public void EndCombat(int playerId)
        {
            _combatSessions.Remove(playerId);
        }

        public bool ApplyAction(int playerId, PlayerActionType action, int? itemIndex = null, string? preferredHand = null)
        {
            lock (_stateLock)
            {
                return actions.ApplyPlayerAction(playerId, action, itemIndex, preferredHand);
            }
        }

        public long EnqueueIncomingAction(int playerId, PlayerActionType action, int? itemIndex = null, string? preferredHand = null)
        {
            long order = Interlocked.Increment(ref _nextActionOrder);
            _pendingActions.Enqueue(new QueuedPlayerAction(order, playerId, action, itemIndex, preferredHand));
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
                    applied = actions.ApplyPlayerAction(queued.PlayerId, queued.Action, queued.ItemIndex, queued.PreferredHand);
                }

                processed.Add(new ProcessedPlayerAction(queued.Order, queued.PlayerId, queued.Action, queued.ItemIndex, queued.PreferredHand, applied));
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
                        MaxHealth = entry.Value.Stats.MaxHealth,
                        InventoryCount = entry.Value.Inventory.Count,
                        CurrentLoad = entry.Value.CurrentLoad,
                        InventoryLimit = entry.Value.Stats.InventoryLimit,
                        Strength = entry.Value.Stats.Strength,
                        Dexterity = entry.Value.Stats.Dexterity,
                        Luck = entry.Value.Stats.Luck,
                        Aggression = entry.Value.Stats.Aggression,
                        Wisdom = entry.Value.Stats.Wisdom,
                        Coins = entry.Value.Stats.Coins,
                        Gold = entry.Value.Stats.Gold,
                        HasTwoHanded = entry.Value.EItems.HasTwoHanded,
                        LeftHand = entry.Value.EItems.LeftHand is null ? null : new WeaponStateDto
                        {
                            Name = entry.Value.EItems.LeftHand.Name,
                            Damage = entry.Value.EItems.LeftHand.Damage,
                            Range = entry.Value.EItems.LeftHand.Range
                        },
                        RightHand = entry.Value.EItems.RightHand is null ? null : new WeaponStateDto
                        {
                            Name = entry.Value.EItems.RightHand.Name,
                            Damage = entry.Value.EItems.RightHand.Damage,
                            Range = entry.Value.EItems.RightHand.Range
                        },
                        InventoryItems = entry.Value.Inventory.Items
                            .Select(item => new InventoryItemDto
                            {
                                Name = item.Name,
                                Symbol = item.Symbol,
                                Weight = item.Weight
                            })
                            .ToList(),
                        IsInCombat = _combatSessions.ContainsKey(entry.Key),
                        Combat = _combatSessions.TryGetValue(entry.Key, out CombatSession? session)
                            ? new CombatStateDto
                            {
                                EnemyName = session.Context.Enemy.Name,
                                EnemyHealth = session.Context.Enemy.Health,
                                EnemyMaxHealth = session.Context.Enemy.MaxHealth,
                                EnemyArmor = session.Context.Enemy.Armor,
                                EnemyDamage = session.Context.Enemy.Damage,
                                LastLog = session.Context.LastLog,
                                ActionNames = CombatSession.ActionNames
                            }
                            : null
                    })
                    .OrderBy(player => player.PlayerId)
                    .ToList();

                var enemies = new List<EnemyStateDto>();
                var itemTiles = new List<ItemTileStateDto>();

                for (int y = 0; y < CurrentRoom.Height; y++)
                {
                    for (int x = 0; x < CurrentRoom.Width; x++)
                    {
                        if (CurrentRoom.Grid[y, x] is not EmptyTile emptyTile)
                            continue;

                        if (emptyTile.HasEnemy && emptyTile.Enemy is not null)
                        {
                            enemies.Add(new EnemyStateDto
                            {
                                Id = emptyTile.Enemy.Id,
                                Name = emptyTile.Enemy.Name,
                                Species = emptyTile.Enemy.Species,
                                X = x,
                                Y = y,
                                Health = emptyTile.Enemy.Health,
                                MaxHealth = emptyTile.Enemy.MaxHealth
                            });
                        }

                        if (emptyTile.Items.Count > 0)
                        {
                            itemTiles.Add(new ItemTileStateDto
                            {
                                X = x,
                                Y = y,
                                ItemCount = emptyTile.Items.Count,
                                Items = emptyTile.Items
                                    .Select(item => new InventoryItemDto
                                    {
                                        Name = item.Name,
                                        Symbol = item.Symbol,
                                        Weight = item.Weight
                                    })
                                    .ToList()
                            });
                        }
                    }
                }

                return new GameStateDto
                {
                    CurrentMapX = CurrentMapX,
                    CurrentMapY = CurrentMapY,
                    CurrentRoomRows = roomRows,
                    Players = players,
                    Enemies = enemies,
                    ItemTiles = itemTiles
                };
            }
        }

        public sealed class CombatSession
        {
            public static List<string> ActionNames { get; } = new List<string>
            {
                "Normal Attack",
                "Stealth Attack",
                "Magical Attack"
            };

            public EmptyTile Tile { get; }
            public FightContext Context { get; }

            public CombatSession(EmptyTile tile, FightContext context)
            {
                Tile = tile;
                Context = context;
            }

            public IFightAction CreateAction(int index)
            {
                return index switch
                {
                    1 => new StealthFightAction(),
                    2 => new MagicalFightAction(),
                    _ => new NormalFightAction()
                };
            }
        }

        public readonly record struct QueuedPlayerAction(long Order, int PlayerId, PlayerActionType Action, int? ItemIndex, string? PreferredHand);
        public readonly record struct ProcessedPlayerAction(long Order, int PlayerId, PlayerActionType Action, int? ItemIndex, string? PreferredHand, bool Applied);
        
    }
}
