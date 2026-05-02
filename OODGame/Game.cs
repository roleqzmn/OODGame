using OODGame.Items;
using OODGame.Entities;
using OODGame.Items.Unequipable;
using OODGame.Items.Weapons;
using OODGame.Map;
using OODGame.Players;
using OODGame.Actions;
using System;
using System.Collections.Generic;

namespace OODGame
{
    public class Game
    {
        public Room CurrentRoom { get; set; }
        public Room[,] Map { get; set; }
        public Player Player { get; set; }
        public int CurrentMapX { get; set; }
        public int CurrentMapY { get; set; }
        public bool IsRunning { get; set; }
        
        public const int RoomWidth = 40;
        public const int RoomHeight = 20;
        private Random _roomRandom = new Random();
        private Actions.Actions actions {  get; set; }

        public Game(GameConfig config)
        {
            Map = new Room[3, 3];
            CurrentMapX = 1;
            CurrentMapY = 1;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Map[y, x] = GenerateConnectedRoom(x, y);
                }
            }

            CurrentRoom = Map[CurrentMapY, CurrentMapX];
            Player = new Player(RoomWidth / 2, RoomHeight / 2, config.PlayerName);
            actions = new Actions.Actions(this);
        }

        private Room GenerateConnectedRoom(int mapX, int mapY)
        {
            var items = new List<Item> { new Unusable1(), new Unusable2(), new Unusable3() };

            int centralRoomWidth  = _roomRandom.Next(12, 18);
            int centralRoomHeight = _roomRandom.Next(10, 14);
            int pathCount         = _roomRandom.Next(3,  7);
            int pathLength        = _roomRandom.Next(80, 150);
            int chamberCount      = _roomRandom.Next(4,  10);
            int chamberMinSize    = _roomRandom.Next(3,  5);
            int chamberMaxSize    = _roomRandom.Next(6,  10);
            int itemCount         = _roomRandom.Next(6,  15);
            int weaponCount       = _roomRandom.Next(3,  8);
            int enemyCount        = _roomRandom.Next(2,  6);

            return new DungeonBuilder(RoomWidth, RoomHeight)
                .CreateFilled()
                .AddCentralRoom(centralRoomWidth, centralRoomHeight)
                .AddPaths(pathCount, pathLength)
                .AddChambers(chamberCount, chamberMinSize, chamberMaxSize)
                .AddItems(items, itemCount)
                .AddWeapons(WeaponFactory.CreateRandom, weaponCount)
                .AddEnemies(EnemyFactory.CreateRandom,  enemyCount)
                .EnsurePassagesConnected(mapX, mapY, 3, 3)
                .Build();
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            Draw.DrawRoom(this);
            Draw.DrawPlayer(this);
            Draw.DrawUI(this);
            Draw.DrawEq(Player);

            IsRunning = true;
            while (IsRunning)
            {
                HandleInput();
            }
        }
        private void HandleInput()
        {
            var key = Console.ReadKey(true).Key;
            actions.Handle(key);
        }

        public void RedrawScreen()
        {
            Console.Clear();
            Draw.DrawRoom(this);
            Draw.DrawPlayer(this);
            Draw.DrawUI(this);
            Draw.DrawEq(Player);
        }

        public void RefreshUI()
        {
            Draw.DrawUI(this);
            Draw.DrawEq(Player);
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
    }
}
