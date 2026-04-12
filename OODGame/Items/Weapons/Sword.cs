using OODGame.Entities;
using OODGame.Items.WeaponTypes;
using OODGame.Players;

namespace OODGame.Items.Weapons
{
    public class Sword : LightWeapon
    {
        public override bool IsTwoHanded => false;

        public Sword()
        {
            Damage = 11;
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
        public override bool CanEquip(Player player) => player.Level >= MinLvl;
    }
}
