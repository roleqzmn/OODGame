using OODGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Players;

namespace OODGame.Items.Weapons
{
    internal class Sword : Weapon
    {
        public override bool IsTwoHanded => false;

        public Sword()
        {
            Damage = 11;
            AttackRate = 2;
            MinLvl = 0;
            Range = 1;
            Symbol = 'S';
            Description = "A steel sword";
            Name = "Sword";
        }

        public override void OnPickedUp(Player player)
        {
            if(player.CanPickup(this))
                player.Pickup(this);
        }
        public override void Attack(Player player, IEnemy enemy)
        {
            throw new NotImplementedException();
        }
        public override void Equip(Player player)
        {
            if (player.EItems.HasTwoHanded)
            {
                player.EItems.HasTwoHanded = false;
                if (player.EItems.RightHand != null)
                    player.Inventory.Add(player.EItems.RightHand);
                player.EItems.RightHand = this;
                Draw.EraseEq();
                Draw.DrawEq(player);
            }
            else
            {
                Console.SetCursorPosition(70, 13);
                Console.Write("Choose the slot: [L] Left hand, [R] Right Hand");
                var choice = Console.ReadKey(true).KeyChar.ToString();

                if (choice == "l")
                {
                    if (player.EItems.LeftHand != null)
                    {
                        player.Inventory.Add(player.EItems.LeftHand);
                    }
                    player.EItems.LeftHand = this;
                    Draw.EraseEq();
                    Draw.DrawEq(player);
                }
                else if (choice == "p")
                {
                    if (player.EItems.RightHand != null)
                    {
                        player.Inventory.Add(player.EItems.RightHand);
                    }
                    player.EItems.RightHand = this;
                    Draw.EraseEq();
                    Draw.DrawEq(player);
                }
                Console.SetCursorPosition(70, 13);
                Console.Write("                                              ");
            }
        }
        public override bool CanEquip(Player player)
        {
            return player.Level >= MinLvl;
        }
    }
}
