using OODGame.Map;
using OODGame.Players;
using OODGame.Logger;
using OODGame.Input;
using OODGame.Networking.Protocol;
using System;
using System.Collections.Generic;

namespace OODGame.Actions
{
    public class Actions
    {
        Game Game { get; set; }
        private readonly IInputSource _inputSource;
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
                PlayerActionType.Interact,
                PlayerActionType.OpenInventory
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

        public bool ApplyPlayerAction(int playerId, PlayerActionType actionType)
        {
            if (!Game.TryGetPlayer(playerId, out Player player))
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
                _turnCounter++;
                if (_turnCounter >= 3)
                {
                    var changedPositions = EnemyMovementService.MoveEnemiesRandomly(Game.CurrentRoom, player.Xpos, player.Ypos);
                    Draw.RedrawChangedPositions(Game, changedPositions);
                    _turnCounter = 0;
                }
            }

            return handled;
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
                    actionType = PlayerActionType.Interact;
                    return true;
                case ConsoleKey.I:
                    actionType = PlayerActionType.OpenInventory;
                    return true;
                case ConsoleKey.J:
                    actionType = PlayerActionType.ShowLog;
                    return true;
                case ConsoleKey.Escape:
                    actionType = PlayerActionType.Quit;
                    return true;
                default:
                    actionType = default;
                    return false;
            }
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
            if (isCombat)
            {
                player.InteractWithTile(tile);
            }
            else if (tile is EmptyTile interactTile && interactTile.Items.Count > 0)
            {
                ConsoleInteractionView.OpenTileItems(player, interactTile, _inputSource);
            }
            if (isCombat)
                Game.RedrawScreen();
            else
                Game.RefreshUI();

            return true;
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
