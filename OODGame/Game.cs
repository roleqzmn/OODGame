using OODGame.Items;
using OODGame.Items.Unequipable;
using OODGame.Items.Weapons;
using OODGame.Map;
using OODGame.Players;
using OODGame.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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

        public Game()
        {
            var items = new List<Item>
            {
                new Unusable1(),
                new Unusable2(),
                new Unusable3()
            };
            var weapons = new List<Weapon>
            {
                new Sword(),
                new Crossbow(),
                new TwoHandedAxe()
            };
            Map = new Room[3, 3];
            CurrentMapX = 1;
            CurrentMapY = 1;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Map[y, x] = GenerateConnectedRoom(x, y, items, weapons);
                }
            }

            CurrentRoom = Map[CurrentMapY, CurrentMapX];
            Player = new Player(RoomWidth / 2, RoomHeight / 2);
            actions = new Actions.Actions(this); 
        }

        private Room GenerateConnectedRoom(int mapX, int mapY, List<Item> items, List<Weapon> weapons)
        {
            int centralRoomWidth = _roomRandom.Next(12, 18);
            int centralRoomHeight = _roomRandom.Next(10, 14);
            int pathCount = _roomRandom.Next(3, 7);
            int pathLength = _roomRandom.Next(80, 150);
            int chamberCount = _roomRandom.Next(4, 10);
            int chamberMinSize = _roomRandom.Next(3, 5);
            int chamberMaxSize = _roomRandom.Next(6, 10);
            int itemCount = _roomRandom.Next(6, 15);
            int weaponCount = _roomRandom.Next(3, 8);

            var builder = new DungeonBuilder(RoomWidth, RoomHeight)
                .CreateFilled()
                .AddCentralRoom(centralRoomWidth, centralRoomHeight)
                .AddPaths(pathCount, pathLength)
                .AddChambers(chamberCount, chamberMinSize, chamberMaxSize)
                .AddItems(items, itemCount)
                .AddWeapons(weapons, weaponCount)
                .EnsurePassagesConnected(mapX, mapY, 3, 3);

            var room = builder.Build();

            return room;
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
            if (actions.actions.TryGetValue(key, out var action))
            {
                action.Invoke();
            }
            else
            {

            }
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
            Console.WriteLine("If you stand on Item tile, press [E] to interact\nPress [I] to open inventory.\nWalk with [W][A][S][D].\nUse arrows to navigate in inventory");
        }
        public static void DrawPlayer(Game game)
        {
            Console.SetCursorPosition(game.Player.Xpos, game.Player.Ypos);
            Console.Write(game.Player.Symbol);
        }
        public static void ErasePlayer(Game game)
        {
            Console.SetCursorPosition(game.Player.Xpos, game.Player.Ypos);
            Console.Write(game.CurrentRoom.Grid[game.Player.Ypos, game.Player.Xpos].Symbol);
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
        public static void DrawItems(List<Item> items)
        {
            Console.SetCursorPosition(45, 11);
            foreach(var item in items)
            {
                Console.Write($"{item.Symbol}, ");
            }
        }
        public static void EraseItems(List<Item> items)
        {
            Console.SetCursorPosition(45, 11);
            Console.Write($"   ");
            foreach (var item in items)
            {
                Console.Write($"   ");
            }
        }
        public static void DrawItem(Item item)
        {
            Console.SetCursorPosition(45, 13);
            Console.Write($"{item.Symbol}-{item.Name}");

            Console.SetCursorPosition(45, 14);
            Console.Write(item.Description);

            Console.SetCursorPosition(45, 15);
            Console.Write("Press E to pick up.");
        }
        public static void EraseItem() 
        {
            Console.SetCursorPosition(45, 13);
            Console.Write("                                    ");

            Console.SetCursorPosition(45, 14);
            Console.Write("                                    ");

            Console.SetCursorPosition(45, 15);
            Console.Write("                                    ");
        }
        public static void DrawInventory()
        {

        }
        public static void DrawItemInv(Item item)
        {
            Console.SetCursorPosition(45, 13);
            Console.Write($"{item.Symbol}-{item.Name}");

            Console.SetCursorPosition(45, 14);
            Console.Write(item.Description);

            Console.SetCursorPosition(45, 15);
            Console.Write("Press E to equip.");
        }
        public static void DrawEq(Player player)
        {
            Console.SetCursorPosition(70, 2);
            if (player.EItems.LeftHand != null){ 
                Console.Write($"Left Hand: {player.EItems.LeftHand.Name},");
                Console.SetCursorPosition(75, 3);
                Console.Write($"Damage:{player.EItems.LeftHand.Damage},");
                Console.SetCursorPosition(75, 4);
                Console.Write($"Attack Speed:{player.EItems.LeftHand.AttackRate},");
                Console.SetCursorPosition(75, 5);
                Console.Write($"Range:{player.EItems.LeftHand.Range}");
            }
            else if(player.EItems.HasTwoHanded) Console.Write("Two Handed");
            else Console.Write("Left Hand: none");
            Console.SetCursorPosition(70, 6);
            if (player.EItems.RightHand != null)
            {
                Console.Write($"Right Hand: {player.EItems.RightHand.Name},");
                Console.SetCursorPosition(75, 7);
                Console.Write($"Damage:{player.EItems.RightHand.Damage},");
                Console.SetCursorPosition(75, 8);
                Console.Write($"Attack Speed:{player.EItems.RightHand.AttackRate},");
                Console.SetCursorPosition(75, 9);
                Console.Write($"Range:{player.EItems.RightHand.Range}");
            }
            else Console.Write("Right Hand: none");

        }
        public static void EraseEq()
        {
            for(int i = 2; i<20; i++)
            {
                Console.SetCursorPosition(70, i);
                Console.Write("                                 ");
            }
        }
        public static void DrawHandChoice()
        {
            Console.SetCursorPosition(70, 13);
            Console.Write("Choose the slot: [L] Left hand, [R] Right Hand");
        }
        public static void EraseHandChoice()
        {
            Console.SetCursorPosition(70, 13);
            Console.Write("                                              ");
        }
    }
}
