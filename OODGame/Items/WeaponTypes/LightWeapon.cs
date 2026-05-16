using OODGame.Entities;

using OODGame.Fight;
using OODGame.Players;

namespace OODGame.Items.WeaponTypes
{
    public abstract class LightWeapon : Weapon, IWeaponCategory, INoiseProfile
    {
        public int NoiseRange => 2;
        public string NoiseCategory => "Light";

        public override void Attack(Player player, Enemy enemy) { }
        public int AcceptAttack(IAttackVisitor visitor, Attributes stats) => visitor.VisitLight(this, stats);
        public int AcceptDefense(IDefenseVisitor visitor, Attributes stats) => visitor.VisitLight(this, stats);
    }
}
