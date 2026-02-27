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
            Damage = 25;
            MinLvl = 0;
            Range = 3;
            Symbol = 'A';
            Description = "Needs two hands";
            Name = "Two-handed Axe";
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

        public override bool CanEquip(Player player)
        {
            return player.Level >= MinLvl;
        }
        public override void Equip(Player player)
        {
            if(player.EItems.HasTwoHanded&& player.EItems.RightHand!=null)
            {
                player.EItems.HasTwoHanded = false;
                player.Inventory.Add(player.EItems.RightHand);
                player.EItems.RightHand = this;
                Draw.EraseEq();
                Draw.DrawEq(player);
            }

        }
    }
}
