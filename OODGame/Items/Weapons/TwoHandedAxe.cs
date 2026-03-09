using OODGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Players;
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
            Range = 1;
            Weight = 2;
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

        public override bool CanEquip(Player player) => player.Level >= MinLvl;
        public override void Equip(Player player)
        {
            player.EItems.HasTwoHanded=true;

            if (player.EItems.RightHand != null)
                player.Pickup(player.EItems.RightHand);
            if(player.EItems.LeftHand != null)
                player.Pickup(player.EItems.LeftHand);

            player.EItems.RightHand = this;
            player.EItems.LeftHand = null;
            Draw.EraseEq();
            Draw.DrawEq(player);
        }
    }
}
