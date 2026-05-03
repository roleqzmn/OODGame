using OODGame.Fight.Actions;
using System;
using System.Collections.Generic;

namespace OODGame.Fight
{
    public static class FightScreen
    {
        private const int Width = 60;

        public static void DrawInitial()
        {
            Console.Clear();
        }

        public static void Draw(FightContext ctx, List<IFightAction> actions, int selectedIdx)
        {
            WriteAt(0, $"=== COMBAT: {ctx.Enemy.Name} ===", Width);
            WriteAt(2, $"Enemy:  {ctx.Enemy.Name,-15} HP: {ctx.Enemy.Health,3}/{ctx.Enemy.MaxHealth,-3}  Armor: {ctx.Enemy.Armor}  Atk: {ctx.Enemy.Damage}", Width);

            var weapon = ctx.Player.EItems.RightHand ?? ctx.Player.EItems.LeftHand;
            WriteAt(4, $"Player: {ctx.Player.Name,-15} HP: {ctx.Player.Stats.Health,3}/{ctx.Player.Stats.MaxHealth,-3}", Width);
            WriteAt(5, $"Weapon: {(weapon != null ? weapon.Name : "none"),-40}", Width);
            WriteAt(7, $"> {ctx.LastLog,-58}", Width);
            WriteAt(9, "Choose action [Up/down = navigate, E = confirm]:", Width);

            for (int i = 0; i < actions.Count; i++)
            {
                Console.SetCursorPosition(2, 10 + i);
                if (i == selectedIdx)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write($"[{i + 1}] {actions[i].Name,-20}");
                Console.ResetColor();
            }
        }

        public static void DrawResult(FightContext ctx)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 5);
            if (ctx.PlayerWon)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  Victory! {ctx.Enemy.Name} was defeated.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  You have been defeated. GAME OVER.");
                Console.ResetColor();
                string? logPath = OODGame.Logger.EventLogger.Instance?.FilePath;
                if (logPath != null)
                {
                    Console.SetCursorPosition(0, 7);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"  Log saved to: {logPath}");
                }
            }
            Console.ResetColor();
            Console.SetCursorPosition(0, 9);
            Console.Write("  Press any key to continue...");
            Console.ReadKey(true);
        }

        private static void WriteAt(int y, string text, int clearWidth)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(text.PadRight(clearWidth));
        }
    }
}
