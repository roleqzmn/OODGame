using OODGame.Entities;
using OODGame.Fight;
using OODGame.Players;

namespace OODGame.Items.WeaponTypes
{
    public abstract class MagicalWeapon : Weapon, IWeaponCategory, INoiseProfile
    {
        public int NoiseRange => 4;
        public string NoiseCategory => "Magical";

        public override void Attack(Player player, Enemy enemy) { }
        public int AcceptAttack(IAttackVisitor visitor, Attributes stats) => visitor.VisitMagical(this, stats);
        public int AcceptDefense(IDefenseVisitor visitor, Attributes stats) => visitor.VisitMagical(this, stats);
    }
}
