using OODGame.Map;
using OODGame.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OODGame
{
    public class Game
    {
        public Room CurrentRoom { get; set; }
        public Room[,] Map { get; set; } //placeholder for later
        public Player.Player Player { get; set; }
        bool IsRunning { get; set; }
        public Game()
        {
            CurrentRoom = new Room();
            Player = new Player.Player();
            Map = new Room[1, 1]; //placeholder for generation
        }
        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            Draw.DrawRoom(this);
            Draw.DrawPlayer(this);
            Draw.DrawUI(this);

            IsRunning = true;
            while (IsRunning)
            {
                HandleInput();
            }
        }
        private void HandleInput()
        {
            var key = Console.ReadKey(true).Key;

            int newX = Player.x_pos;
            int newY = Player.y_pos;
            Tile tile = CurrentRoom.Grid[Player.y_pos, Player.x_pos];
            switch (key)
            {
                case ConsoleKey.W: newY--; break;
                case ConsoleKey.S: newY++; break;
                case ConsoleKey.A: newX--; break;
                case ConsoleKey.D: newX++; break;
                case ConsoleKey.E: if (tile.CanInteract()) tile.Interact(); break;
                case ConsoleKey.Escape: IsRunning = false; return;  
            }

            TryMove(newX, newY);
        }

        private void TryMove(int newX, int newY)
        {
            if (newX < 0 || newX >= CurrentRoom.Width || newY < 0 || newY >= CurrentRoom.Height) //will add moving to other rooms later
                return;

            if (CurrentRoom.Grid[newY, newX].CanEnter())
            {
                Draw.ErasePlayer(this);

                Player.x_pos = newX;
                Player.y_pos = newY;

                Draw.DrawPlayer(this);
            }
        }
    }
    public class Draw
    {
        public static void DrawRoom(Game game)
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < game.CurrentRoom.Height; y++)
            {
                for (int x = 0; x < game.CurrentRoom.Width; x++)
                {
                    Console.Write(game.CurrentRoom.Grid[y, x].Symbol);
                }
                Console.WriteLine();
            }
        }
        public static void DrawPlayer(Game game)
        {
            Console.SetCursorPosition(game.Player.x_pos, game.Player.y_pos);
            Console.Write(game.Player.Symbol);
        }

        public static void ErasePlayer(Game game)
        {
            Console.SetCursorPosition(game.Player.x_pos, game.Player.y_pos);
            Console.Write(game.CurrentRoom.Grid[game.Player.y_pos, game.Player.x_pos].Symbol);
        }
        public static void DrawUI(Game game)
        {
            int X = 45;

            Console.SetCursorPosition(X, 0);
            Console.Write($"--- {game.Player.Name} ---");

            Console.SetCursorPosition(X, 2);
            Console.Write($"Health: {game.Player.Stats.Health}/{game.Player.Stats.MaxHealth}    ");

            Console.SetCursorPosition(X, 3);
            Console.Write($"Load: {game.Player.Stats.CurrentLoad}/{game.Player.Stats.InventoryLimit}    ");

            Console.SetCursorPosition(X, 4);
            Console.Write($"Strength: {game.Player.Stats.Strength}    ");

            Console.SetCursorPosition(X, 5);
            Console.Write($"Dexterity: {game.Player.Stats.Dexterity}    ");

            Console.SetCursorPosition(X, 6);
            Console.Write($"Luck: {game.Player.Stats.Luck}    ");

            Console.SetCursorPosition(X, 7);
            Console.Write($"Agression: {game.Player.Stats.Aggression}    ");

            Console.SetCursorPosition(X, 8);
            Console.Write($"Wisdom: {game.Player.Stats.Wisdom}    ");

            Console.SetCursorPosition(X, 9);
            Console.Write($"Coins: {game.Player.Stats.Coins} | Gold: {game.Player.Stats.Gold}    ");

        }

    }
}
