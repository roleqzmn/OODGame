using OODGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Players;

namespace OODGame.Items.Weapons
{
    internal class Crossbow : OneHandedWeapon
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
        public override void Attack(Player player, Enemy enemy)
        {
            throw new NotImplementedException();
        }
        public override bool CanEquip(Player player) => player.Level >= MinLvl;
        
    }
}
