using OODGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Players;

namespace OODGame.Items.Weapons
{
    internal class Crossbow : Weapon
    {
        public override bool IsTwoHanded => false;

        public Crossbow()
        {
            Damage = 8;
            MinLvl = 0;
            AttackRate = 3;
            Range = 3;
            Weight = 1;
            Symbol = 'C';
            Description = "Long-range weapon";
            Name = "Crossbow";
        }

        public override void OnPickedUp(Player player)
        {
            if (player.CanPickup(this))
                player.Pickup(this);
        }
        public override void Attack(Player player, IEnemy enemy)
        {
            throw new NotImplementedException();
        }

        public override bool CanEquip(Player player) => player.Level >= MinLvl;
        public override void Equip(Player player)
        {
            if(player.EItems.HasTwoHanded)
            {
                player.EItems.HasTwoHanded = false;
                if(player.EItems.RightHand != null)
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

                if(choice == "l")
                {
                    if(player.EItems.LeftHand != null)
                    {
                        player.Pickup(player.EItems.LeftHand);
                    }
                    player.EItems.LeftHand = this;
                    Draw.EraseEq();
                    Draw.DrawEq(player);
                }
                else if(choice == "p")
                {
                    if(player.EItems.RightHand != null)
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
    }
}
