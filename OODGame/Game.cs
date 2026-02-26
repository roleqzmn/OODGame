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
        Room CurrentRoom { get; set; }
        Room[,] Map { get; set; } //placeholder for later
        Player.Player Player { get; set; }
        bool IsRunning { get; set; }
        public Game()
        {
            CurrentRoom = new Room(); //one room for now so i can do it like that
            Player = new Player.Player();
            Map = new Room[1, 1]; //placeholder for generation
        }
        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            DrawRoom();
            DrawPlayer();

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
            Tile tile = CurrentRoom.Grid[Player.x_pos, Player.y_pos];
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
            if (newX < 0 || newX >= CurrentRoom.Width || newY < 0 || newY >= CurrentRoom.Height)
                return;

            if (CurrentRoom.Grid[newY, newX].CanEnter())
            {
                ErasePlayer();

                Player.x_pos = newX;
                Player.y_pos = newY;

                DrawPlayer();
            }
        }
        private void DrawRoom()
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < CurrentRoom.Height; y++)
            {
                for (int x = 0; x < CurrentRoom.Width; x++)
                {
                    Console.Write(CurrentRoom.Grid[y, x].Symbol);
                }
                Console.WriteLine();
            }
        }

        private void DrawPlayer()
        {
            Console.SetCursorPosition(Player.x_pos, Player.y_pos);
            Console.Write(Player.Symbol);
        }

        private void ErasePlayer()
        {
            Console.SetCursorPosition(Player.x_pos, Player.y_pos);
            Console.Write(CurrentRoom.Grid[Player.y_pos, Player.x_pos].Symbol);
        }
    }
}
