using OODGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Players;

namespace OODGame.Items.Weapons
{
    internal class Sword : OneHandedWeapon
    {
        public override bool IsTwoHanded => false;

        public Sword()
        {
            Damage = 11;
            AttackRate = 2;
            MinLvl = 0;
            Range = 1;
            Weight = 1;
            Symbol = 'S';
            Description = "A steel sword";
            Name = "Sword";
        }

        public override void OnPickedUp(Player player)
        {
            if(player.CanPickup(this))
                player.Pickup(this);
        }
        public override void Attack(Player player, Enemy enemy)
        {
            throw new NotImplementedException();
        }
        public override bool CanEquip(Player player) => player.Level >= MinLvl;
    }
}
