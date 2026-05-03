using OODGame.Items;
using OODGame.Dungeon;
using OODGame.Entities;
using OODGame.Items;
using OODGame.Items.Unequipable;
using OODGame.Items.Weapons;
using OODGame.Map;
using OODGame.Players;
using OODGame.Actions;
using OODGame.Logger;
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
        private readonly IDungeonTheme _theme;
        private readonly Random _roomRandom = new Random();
        private Actions.Actions actions { get; set; }

        public Game(GameConfig config)
        {
            EventLogger.GetInstance(config.LogPath);
            _theme = DungeonThemeFactory.Create(config.DungeonTheme);

            Map = new Room[3, 3];
            CurrentMapX = 1;
            CurrentMapY = 1;
            // Artefakt trafia tylko do pokoju środkowego (1,1)
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    Map[y, x] = GenerateConnectedRoom(x, y, isCenter: x == 1 && y == 1);

            CurrentRoom = Map[CurrentMapY, CurrentMapX];
            Player = new Player(RoomWidth / 2, RoomHeight / 2, config.PlayerName);
            actions = new Actions.Actions(this);
        }

        private Room GenerateConnectedRoom(int mapX, int mapY, bool isCenter)
        {
            return _theme.GenerationStrategy.Generate(
                new DungeonBuilder(RoomWidth, RoomHeight),
                mapX, mapY,
                _theme.EnemyFactory,
                _theme.GetPossibleItems(),
                _theme.CreateArtifact(),
                placeArtifact: isCenter);
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Draw.DrawIntro(_theme);

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
