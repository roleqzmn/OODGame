using OODGame.Entities;
using OODGame.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OODGame.Items
{
    public abstract class Weapon : Item
    {
        public Int16 Damage { get; set; }
        public Int16 MinLvl { get; set; }
        public Int16 AttackRate { get; set; }
        public Int16 Range { get; set; }
        public abstract bool IsTwoHanded { get; }
        public abstract void Attack(Player player, Enemy enemy);
    }
    public abstract class OneHandedWeapon : Weapon
    {
        public override void Equip(Player player)
        {
            if (player.EItems.HasTwoHanded)
            {
                player.EItems.HasTwoHanded = false;
                if (player.EItems.RightHand != null)
                    player.Pickup(player.EItems.RightHand);
                player.EItems.RightHand = null;
            }
            Console.SetCursorPosition(70, 13);
            Console.Write("Choose the slot: [L] Left hand, [R] Right Hand");
            var choice = Console.ReadKey(true).KeyChar.ToString().ToUpper();

            if (choice == "L")
            {
                if (player.EItems.LeftHand != null)
                {
                    player.Pickup(player.EItems.LeftHand);
                }
                player.EItems.LeftHand = this;
                Draw.EraseEq();
                Draw.DrawEq(player);
            }
            else if (choice == "R")
            {
                if (player.EItems.RightHand != null)
                {
                    player.Pickup(player.EItems.RightHand);
                }
                player.EItems.RightHand = this;
                Draw.EraseEq();
                Draw.DrawEq(player);
            }
            Console.SetCursorPosition(70, 13);
            Console.Write("                                              ");
        }
    }
    public abstract class TwoHandedWeapon : Weapon
    {
        public override void Equip(Player player)
        {
            player.EItems.HasTwoHanded = true;

            if (player.EItems.RightHand != null)
                player.Pickup(player.EItems.RightHand);
            if (player.EItems.LeftHand != null)
                player.Pickup(player.EItems.LeftHand);

            player.EItems.RightHand = this;
            player.EItems.LeftHand = null;
            Draw.EraseEq();
            Draw.DrawEq(player);
        }
    }
}
