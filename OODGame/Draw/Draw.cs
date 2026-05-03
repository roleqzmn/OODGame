using OODGame.Items;
using OODGame.Dungeon;
using OODGame.Map;
using OODGame.Players;
using System;
using System.Collections.Generic;

namespace OODGame
{
    public class Draw
    {
        public static void DrawIntro(IDungeonTheme theme)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 3);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  === {theme.Name.ToUpper()} ===");
            Console.ResetColor();
            Console.WriteLine();
            foreach (var line in theme.IntroMessage.Split('\n'))
                Console.WriteLine($"  {line}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  Press any key to begin...");
            Console.ResetColor();
            Console.ReadKey(true);
        }


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
            Console.WriteLine("If you stand on Item tile, press [E] to interact [ESC] to quit\nPress [I] to open inventory.\nWalk with [W][A][S][D].\nUse arrows to navigate in inventory");
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
            Console.Write($"Load: {game.Player.CurrentLoad}/{game.Player.Stats.InventoryLimit}    ");

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
            foreach (var item in items)
                Console.Write($"{item.Symbol}, ");
        }

        public static void EraseItems(List<Item> items)
        {
            Console.SetCursorPosition(45, 11);
            Console.Write("   ");
            foreach (var item in items)
                Console.Write("   ");
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

        public static void DrawItemInv(Item item, Player player)
        {
            Console.SetCursorPosition(45, 13);
            Console.Write($"{item.Symbol}-{item.Name}");

            Console.SetCursorPosition(45, 14);
            Console.Write(item.Description);

            Console.SetCursorPosition(45, 15);
            if (item.CanEquip(player))
                Console.Write("Press E to equip.        ");
            else if (item is Weapon w)
                Console.Write($"Req. level: {w.MinLvl}    ");
            else
                Console.Write("Cannot equip.            ");
        }

        public static void DrawEq(Player player)
        {
            Console.SetCursorPosition(70, 2);
            if (player.EItems.LeftHand != null)
            {
                Console.Write($"Left Hand: {player.EItems.LeftHand.Name},");
                Console.SetCursorPosition(75, 3);
                Console.Write($"Damage:{player.EItems.LeftHand.Damage},");
                Console.SetCursorPosition(75, 4);
                Console.Write($"Range:{player.EItems.LeftHand.Range}");
            }
            else if (player.EItems.HasTwoHanded) Console.Write("Two Handed");
            else Console.Write("Left Hand: none");

            Console.SetCursorPosition(70, 6);
            if (player.EItems.RightHand != null)
            {
                Console.Write($"Right Hand: {player.EItems.RightHand.Name},");
                Console.SetCursorPosition(75, 7);
                Console.Write($"Damage:{player.EItems.RightHand.Damage},");
                Console.SetCursorPosition(75, 8);
                Console.Write($"Range:{player.EItems.RightHand.Range}");
            }
            else Console.Write("Right Hand: none");
        }

        public static void EraseEq()
        {
            for (int i = 2; i <= 8; i++)
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
