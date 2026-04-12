using OODGame.Entities;
using OODGame.Items.WeaponTypes;
using OODGame.Players;

namespace OODGame.Items.Weapons
{
    public class TwoHandedAxe : HeavyWeapon
    {
        public override bool IsTwoHanded => true;

        public TwoHandedAxe()
        {
            Damage = 25;
            MinLvl = 0;
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
        public override bool CanEquip(Player player) => player.Level >= MinLvl;
        
    }
}
