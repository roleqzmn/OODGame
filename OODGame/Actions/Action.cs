using OODGame.Map;
using OODGame.Map;
using OODGame.Players;
using OODGame.Items;
using OODGame.Logger;
using OODGame.Input;
using OODGame.Networking.Protocol;
using OODGame.Fight.Actions;
using OODGame.Events;
using System;
using System.Collections.Generic;

namespace OODGame.Actions
{
    public class Actions
    {
        Game Game { get; set; }
        private readonly IInputSource _inputSource;
        private readonly PlayerActions _playerActions = new PlayerActions();
        private readonly HashSet<PlayerActionType> _turnActions;
        private int _turnCounter;
        public Actions(Game game, IInputSource inputSource)
        {
            Game = game;
            _inputSource = inputSource;
            _turnActions = new HashSet<PlayerActionType>
            {
                PlayerActionType.MoveUp,
                PlayerActionType.MoveLeft,
                PlayerActionType.MoveDown,
                PlayerActionType.MoveRight,
                PlayerActionType.PickupItem,
                PlayerActionType.EquipItem,
                PlayerActionType.Attack,
                PlayerActionType.Interact,
                PlayerActionType.OpenInventory,
                PlayerActionType.DropItem
            };
            _turnCounter = 0;
        }

        public void Handle(ConsoleKey key)
        {
            if (TryMapKeyToAction(key, out PlayerActionType actionType))
            {
                Game.ApplyAction(Game.LocalPlayerId, actionType);
            }
            else
            {
                EventLogger.Instance?.LogEvent("Pressed an unknown key.");
            }
        }

        public bool ApplyPlayerAction(int playerId, PlayerActionType actionType, int? itemIndex = null, string? preferredHand = null)
        {
            if (!Game.TryGetPlayer(playerId, out Player player))
                return false;

            if (!Game.HasLocalPlayer && !IsServerSafeAction(actionType))
                return false;

            if (!Game.HasLocalPlayer && Game.IsPlayerInCombat(playerId) && actionType is not PlayerActionType.Attack and not PlayerActionType.Quit)
                return false;

            bool handled;
            switch (actionType)
            {
                case PlayerActionType.MoveUp:
                    handled = TryMove(playerId, player.Xpos, player.Ypos - 1);
                    break;
                case PlayerActionType.MoveDown:
                    handled = TryMove(playerId, player.Xpos, player.Ypos + 1);
                    break;
                case PlayerActionType.MoveLeft:
                    handled = TryMove(playerId, player.Xpos - 1, player.Ypos);
                    break;
                case PlayerActionType.MoveRight:
                    handled = TryMove(playerId, player.Xpos + 1, player.Ypos);
                    break;
                case PlayerActionType.PickupItem:
                    handled = PickupItemFromTile(playerId, itemIndex ?? 0);
                    break;
                case PlayerActionType.EquipItem:
                    handled = EquipInventoryItem(playerId, itemIndex ?? 0, preferredHand);
                    break;
                case PlayerActionType.Attack:
                    handled = AttackEnemy(playerId, itemIndex ?? 0);
                    break;
                case PlayerActionType.Interact:
                    handled = InteractWithTile(playerId);
                    break;
                case PlayerActionType.OpenInventory:
                    handled = OpenPlayerInventory(playerId);
                    break;
                case PlayerActionType.ShowLog:
                    ShowFullLog();
                    handled = true;
                    break;
                case PlayerActionType.DropItem:
                    handled = DropInventoryItem(playerId, itemIndex ?? 0);
                    break;
                case PlayerActionType.Quit:
                    QuitGame();
                    handled = true;
                    break;
                default:
                    handled = false;
                    break;
            }

            if (handled && _turnActions.Contains(actionType) && Game.IsRunning)
            {
                bool canAdvanceWorldTick = !Game.HasActiveCombats();
                if (canAdvanceWorldTick)
                {
                    _turnCounter++;
                    if (_turnCounter >= 3)
                    {
                        var changedPositions = EnemyMovementService.MoveEnemiesRandomly(Game.CurrentRoom, player.Xpos, player.Ypos);
                        Draw.RedrawChangedPositions(Game, changedPositions);
                        _turnCounter = 0;
                    }
                }
            }

            return handled;
        }

        private static bool IsServerSafeAction(PlayerActionType actionType)
        {
            return actionType is PlayerActionType.MoveUp
                or PlayerActionType.MoveDown
                or PlayerActionType.MoveLeft
                or PlayerActionType.MoveRight
                or PlayerActionType.PickupItem
                or PlayerActionType.EquipItem
                or PlayerActionType.Attack
                or PlayerActionType.Interact
                or PlayerActionType.DropItem
                or PlayerActionType.Quit;
        }

        private static bool TryMapKeyToAction(ConsoleKey key, out PlayerActionType actionType)
        {
            switch (key)
            {
                case ConsoleKey.W:
                    actionType = PlayerActionType.MoveUp;
                    return true;
                case ConsoleKey.S:
                    actionType = PlayerActionType.MoveDown;
                    return true;
                case ConsoleKey.A:
                    actionType = PlayerActionType.MoveLeft;
                    return true;
                case ConsoleKey.D:
                    actionType = PlayerActionType.MoveRight;
                    return true;
                case ConsoleKey.E:
                    actionType = PlayerActionType.PickupItem;
                    return true;
                case ConsoleKey.R:
                    actionType = PlayerActionType.EquipItem;
                    return true;
                case ConsoleKey.F:
                    actionType = PlayerActionType.Attack;
                    return true;
                case ConsoleKey.I:
                    actionType = PlayerActionType.OpenInventory;
                    return true;
                case ConsoleKey.J:
                    actionType = PlayerActionType.ShowLog;
                    return true;
                case ConsoleKey.Q:
                    actionType = PlayerActionType.DropItem;
                    return true;
                case ConsoleKey.Escape:
                    actionType = PlayerActionType.Quit;
                    return true;
                default:
                    actionType = default;
                    return false;
            }
        }

        private bool AttackEnemy(int playerId, int attackIndex)
        {
            if (!Game.TryGetPlayer(playerId, out Player player))
                return false;

            if (!Game.HasLocalPlayer && Game.TryGetCombatSession(playerId, out Game.CombatSession? combatSession))
                return ResolveServerCombatTurn(playerId, player, combatSession, attackIndex);

            if (!TryFindAttackTargetTile(player, out EmptyTile? combatTile) || combatTile is null)
                return false;

            if (!Game.HasLocalPlayer)
            {
                Game.TryStartCombat(playerId, combatTile);
                if (Game.TryGetCombatSession(playerId, out Game.CombatSession? startedSession))
                    return ResolveServerCombatTurn(playerId, player, startedSession, attackIndex);
                return false;
            }

            player.InteractWithTile(combatTile);
            Game.RedrawScreen();
            return true;
        }

        private bool TryFindAttackTargetTile(Player player, out EmptyTile? tile)
        {
            tile = null;

            if (Game.CurrentRoom.Grid[player.Ypos, player.Xpos] is EmptyTile current && current.HasEnemy)
            {
                tile = current;
                return true;
            }

            ReadOnlySpan<(int dx, int dy)> offsets =
            [
                (0, -1),
                (1, 0),
                (0, 1),
                (-1, 0)
            ];

            foreach (var (dx, dy) in offsets)
            {
                int x = player.Xpos + dx;
                int y = player.Ypos + dy;

                if (x < 0 || x >= Game.RoomWidth || y < 0 || y >= Game.RoomHeight)
                    continue;

                if (Game.CurrentRoom.Grid[y, x] is EmptyTile adjacent && adjacent.HasEnemy)
                {
                    tile = adjacent;
                    return true;
                }
            }

            return false;
        }

        private bool ResolveServerCombatTurn(int playerId, Player player, Game.CombatSession combatSession, int attackIndex)
        {
            if (!combatSession.Tile.HasEnemy || combatSession.Tile.Enemy is null)
            {
                Game.EndCombat(playerId);
                return false;
            }

            int bounded = Math.Clamp(attackIndex, 0, 2);
            IFightAction action = combatSession.CreateAction(bounded);
            action.Execute(combatSession.Context);

            var enemy = combatSession.Context.Enemy;
            if (enemy.Health <= 0)
            {
                player.EventBus?.Publish(new EnemyDeathEvent(enemy.Id, enemy.Species));
                player.EventBus?.Unsubscribe(enemy);
                enemy.ClearSpatialContext();
                combatSession.Tile.RemoveEnemy();
                EventLogger.Instance?.LogEvent($"{player.Name} defeated {enemy.Name}.");
                Game.EndCombat(playerId);
                return true;
            }

            if (player.Stats.Health <= 0)
            {
                Game.EndCombat(playerId);
                Game.RemovePlayer(playerId);
                EventLogger.Instance?.LogEvent($"{player.Name} was defeated.");
            }

            return true;
        }

        private void ShowFullLog()
        {
            EventLogger.Instance?.ViewAllLogs();
            Draw.RedrawFull(Game);
        }

        public void QuitGame()
        {
            EventLogger.Instance?.LogEvent("Game exited.");
            Game.IsRunning=false;
        }
        private bool InteractWithTile(int playerId)
        {
            if (!Game.TryGetPlayer(playerId, out Player player))
                return false;

            Tile tile = Game.CurrentRoom.Grid[player.Ypos, player.Xpos];
            bool isCombat = tile is EmptyTile emptyTile && emptyTile.HasEnemy;

            if (!Game.HasLocalPlayer)
            {
                if (isCombat && tile is EmptyTile serverCombatTile)
                    return Game.TryStartCombat(playerId, serverCombatTile);
                return false;
            }

            if (isCombat)
            {
                player.InteractWithTile(tile);
                Game.RedrawScreen();
                return true;
            }

            if (tile is EmptyTile interactTile && interactTile.Items.Count > 0)
            {
                ConsoleInteractionView.OpenTileItems(player, interactTile, _inputSource);
                Game.RefreshUI();
                return true;
            }

            return false;
        }

        private bool PickupItemFromTile(int playerId, int itemIndex)
        {
            if (!Game.TryGetPlayer(playerId, out Player player))
                return false;

            Tile tile = Game.CurrentRoom.Grid[player.Ypos, player.Xpos];
            if (tile is not EmptyTile emptyTile || emptyTile.Items.Count == 0)
                return false;

            int boundedIndex = Math.Clamp(itemIndex, 0, emptyTile.Items.Count - 1);
            PlayerActionResult result = emptyTile.PickupItem(player, boundedIndex);

            if (result.Success && Game.HasLocalPlayer)
                Game.RefreshUI();

            return result.Success;
        }

        private bool EquipInventoryItem(int playerId, int itemIndex, string? preferredHand)
        {
            if (!Game.TryGetPlayer(playerId, out Player player))
                return false;

            if (player.Inventory.Count == 0)
                return false;

            int boundedIndex = Math.Clamp(itemIndex, 0, player.Inventory.Count - 1);
            Item item = player.Inventory[boundedIndex];
            if (!item.CanEquip(player))
                return false;

            bool equipped;
            if (item is Weapon weapon)
            {
                WeaponHand hand = string.Equals(preferredHand, "left", StringComparison.OrdinalIgnoreCase)
                    ? WeaponHand.Left
                    : WeaponHand.Right;
                equipped = player.EquipWeapon(weapon, hand);
            }
            else
            {
                equipped = item.Equip(player);
            }

            if (!equipped)
                return false;

            player.Inventory.RemoveItem(item);
            EventLogger.Instance?.LogEvent($"{player.Name} equipped {item.Name}.");

            if (Game.HasLocalPlayer)
                Game.RefreshUI();

            return true;
        }

        private bool DropInventoryItem(int playerId, int itemIndex)
        {
            if (!Game.TryGetPlayer(playerId, out Player player))
                return false;

            if (player.Inventory.Count == 0)
                return false;

            int boundedIndex = Math.Clamp(itemIndex, 0, player.Inventory.Count - 1);

            Tile tile = Game.CurrentRoom.Grid[player.Ypos, player.Xpos];
            PlayerActionResult result = _playerActions.DropFromInventory(player, tile, boundedIndex);

            if (result.Success && Game.HasLocalPlayer)
                Game.RefreshUI();

            return result.Success;
        }

        private bool OpenPlayerInventory(int playerId)
        {
            if (!Game.TryGetPlayer(playerId, out Player player))
                return false;

            Tile tile = Game.CurrentRoom.Grid[player.Ypos, player.Xpos];
            ConsoleInteractionView.OpenInventory(player, tile, _inputSource);
            Game.RefreshUI();
            return true;
        }
        private bool TryMove(int playerId, int newX, int newY)
        {
            if (!Game.TryGetPlayer(playerId, out Player player))
                return false;

            if (!Game.HasLocalPlayer && (newX < 0 || newX >= Game.RoomWidth || newY < 0 || newY >= Game.RoomHeight))
            {
                EventLogger.Instance?.LogEvent($"{player.Name} tried to leave shared room bounds.");
                return false;
            }

            if (newX == player.Xpos && newY == player.Ypos)
            {
                return false;
            }
            if (newX < 0 && Game.CurrentMapX > 0)
            {
                Game.CurrentMapX--;
                Game.CurrentRoom = Game.Map[Game.CurrentMapY, Game.CurrentMapX];
                player.EventBus = Game.CurrentRoom.EventBus;
                Draw.ErasePlayer(Game, player);
                player.Xpos = Game.RoomWidth - 2;
                player.Ypos = newY;
                Draw.DrawRoom(Game);
                Draw.DrawPlayers(Game);
                return true;
            }

            if (newX >= Game.RoomWidth && Game.CurrentMapX < 2)
            {
                Game.CurrentMapX++;
                Game.CurrentRoom = Game.Map[Game.CurrentMapY, Game.CurrentMapX];
                player.EventBus = Game.CurrentRoom.EventBus;
                Draw.ErasePlayer(Game, player);
                player.Xpos = 1;
                player.Ypos = newY;
                Draw.DrawRoom(Game);
                Draw.DrawPlayers(Game);
                return true;
            }

            if (newY < 0 && Game.CurrentMapY > 0)
            {
                Game.CurrentMapY--;
                Game.CurrentRoom = Game.Map[Game.CurrentMapY, Game.CurrentMapX];
                player.EventBus = Game.CurrentRoom.EventBus;
                Draw.ErasePlayer(Game, player);
                player.Xpos = newX;
                player.Ypos = Game.RoomHeight - 2;
                Draw.DrawRoom(Game);
                Draw.DrawPlayers(Game);
                return true;
            }

            if (newY >= Game.RoomHeight && Game.CurrentMapY < 2)
            {
                Game.CurrentMapY++;
                Game.CurrentRoom = Game.Map[Game.CurrentMapY, Game.CurrentMapX];
                player.EventBus = Game.CurrentRoom.EventBus;
                Draw.ErasePlayer(Game, player);
                player.Xpos = newX;
                player.Ypos = 1;
                Draw.DrawRoom(Game);
                Draw.DrawPlayers(Game);
                return true;
            }
            if (newX >= 0 && newX < Game.RoomWidth && newY >= 0 && newY < Game.RoomHeight)
            {
                Tile targetTile = Game.CurrentRoom.Grid[newY, newX];
                if (targetTile.CanEnter())
                {
                    Draw.ErasePlayer(Game, player);
                    player.Xpos = newX;
                    player.Ypos = newY;

                    if (targetTile is EmptyTile emptyTile && emptyTile.HasEnemy)
                    {
                        if (!Game.HasLocalPlayer)
                        {
                            Game.TryStartCombat(playerId, emptyTile);
                            Draw.DrawPlayers(Game);
                            return true;
                        }

                        player.InteractWithTile(targetTile);
                        Game.RedrawScreen();
                    }
                    else
                    {
                        Draw.DrawPlayers(Game);
                    }

                    return true;
                }
                else
                {
                    EventLogger.Instance?.LogEvent($"Player tried to walk into a wall at ({newX},{newY}).");
                }
            }

            return false;
        }
    }
}
