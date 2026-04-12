using OODGame.Entities;
using OODGame.Items.WeaponTypes;
using OODGame.Players;

namespace OODGame.Items.Weapons
{
    public class Crossbow : LightWeapon
    {
        public override bool IsTwoHanded => false;

        public Crossbow()
        {
            Damage = 8;
            MinLvl = 0;
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
        public override bool CanEquip(Player player) => player.Level >= MinLvl;
        
    }
}
