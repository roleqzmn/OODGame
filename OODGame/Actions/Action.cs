using OODGame.Map;
using OODGame.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OODGame.Actions
{
    public class Actions
    {
        Game Game { get; set; }
        public Dictionary<ConsoleKey, Action> actions;
        public Actions(Game game)
        {
            Game = game;
            actions = new Dictionary<ConsoleKey, Action>
            {
                { ConsoleKey.W, () => TryMove(Game.Player.Xpos, Game.Player.Ypos - 1) },
                { ConsoleKey.S, () => TryMove(Game.Player.Xpos, Game.Player.Ypos + 1) },
                { ConsoleKey.A, () => TryMove(Game.Player.Xpos - 1, Game.Player.Ypos) },
                { ConsoleKey.D, () => TryMove(Game.Player.Xpos + 1,  Game.Player.Ypos) },
                { ConsoleKey.E, () => InteractWithTile() },
                { ConsoleKey.I, () => OpenPlayerInventory() },
                { ConsoleKey.Escape, () => Game.IsRunning = false }
            };
        }

        private void InteractWithTile()
        {
            Tile tile = Game.CurrentRoom.Grid[Game.Player.Ypos, Game.Player.Xpos];
            if (tile.CanInteract())
                tile.Interact(Game.Player);
        }

        private void OpenPlayerInventory()
        {
            Tile tile = Game.CurrentRoom.Grid[Game.Player.Ypos, Game.Player.Xpos];
            Game.Player.OpenInventory(tile);
        }
        private void TryMove(int newX, int newY)
        {
            if (newX == Game.Player.Xpos && newY == Game.Player.Ypos)
            {
                return;
            }
            if (newX < 0 && Game.CurrentMapX > 0)
            {
                Game.CurrentMapX--;
                Game.CurrentRoom = Game.Map[Game.CurrentMapY, Game.CurrentMapX];
                Game.Player.Xpos = Game.RoomWidth - 2;
                Game.Player.Ypos = newY;
                Draw.ErasePlayer(Game);
                Draw.DrawRoom(Game);
                Draw.DrawPlayer(Game);
                return;
            }

            if (newX >= Game.RoomWidth && Game.CurrentMapX < 2)
            {
                Game.CurrentMapX++;
                Game.CurrentRoom = Game.Map[Game.CurrentMapY, Game.CurrentMapX];
                Game.Player.Xpos = 1;
                Game.Player.Ypos = newY;
                Draw.ErasePlayer(Game);
                Draw.DrawRoom(Game);
                Draw.DrawPlayer(Game);
                return;
            }

            if (newY < 0 && Game.CurrentMapY > 0)
            {
                Game.CurrentMapY--;
                Game.CurrentRoom = Game.Map[Game.CurrentMapY, Game.CurrentMapX];
                Game.Player.Xpos = newX;
                Game.Player.Ypos = Game.RoomHeight - 2;
                Draw.ErasePlayer(Game);
                Draw.DrawRoom(Game);
                Draw.DrawPlayer(Game);
                return;
            }

            if (newY >= Game.RoomHeight && Game.CurrentMapY < 2)
            {
                Game.CurrentMapY++;
                Game.CurrentRoom = Game.Map[Game.CurrentMapY, Game.CurrentMapX];
                Game.Player.Xpos = newX;
                Game.Player.Ypos = 1;
                Draw.ErasePlayer(Game);
                Draw.DrawRoom(Game);
                Draw.DrawPlayer(Game);
                return;
            }
            if (newX >= 0 && newX < Game.RoomWidth && newY >= 0 && newY < Game.RoomHeight)
            {
                if (Game.CurrentRoom.Grid[newY, newX].CanEnter())
                {
                    Draw.ErasePlayer(Game);

                    Game.Player.Xpos = newX;
                    Game.Player.Ypos = newY;

                    Draw.DrawPlayer(Game);
                }
            }
        }
    }
}
