using OODGame.Entities;
using OODGame.Fight;
using OODGame.Players;

namespace OODGame.Items.WeaponTypes
{
    public abstract class MagicalWeapon : Weapon, IWeaponCategory
    {
        public override void Attack(Player player, Enemy enemy) { }
        public int AcceptAttack(IAttackVisitor visitor, Attributes stats) => visitor.VisitMagical(this, stats);
        public int AcceptDefense(IDefenseVisitor visitor, Attributes stats) => visitor.VisitMagical(this, stats);
    }
}
