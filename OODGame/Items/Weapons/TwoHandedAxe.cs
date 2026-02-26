using OODGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OODGame.Items.Weapons
{
    internal class TwoHandedAxe : Weapon
    {   
        public override bool IsTwoHanded => true;

        public TwoHandedAxe()
        {
            Damage = 25;
            MinLvl = 0;
            AttackRate = 1;
            Symbol = 'A';
            Description = "Needs two hands";
            Name = "Two-handed Axe";
        }

        public override void OnPickedUp(Player.Player player)
        {
        }
        public override void Attack(Player.Player player, IEnemy enemy)
        {
        }

        public override bool CanEquip(Player.Player player)
        {
            return player.Level >= MinLvl;
        }
    }
}
