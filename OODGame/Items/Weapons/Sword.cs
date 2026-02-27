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
        public override bool IsTwoHanded => true;

        public Sword()
        {
            Damage = 11;
            AttackRate = 2;
            MinLvl = 0;
            Range = 1;
            Symbol = 'A';
            Description = "Needs two hands";
            Name = "Two-handed Axe";
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
            throw new NotImplementedException();
        }
        public override bool CanEquip(Player player)
        {
            return player.Level >= MinLvl;
        }
    }
}
