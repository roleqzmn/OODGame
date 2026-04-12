using OODGame.Entities;
using OODGame.Fight;
using OODGame.Players;

namespace OODGame.Items.WeaponTypes
{
    public abstract class HeavyWeapon : Weapon, IWeaponCategory
    {
        public override void Attack(Player player, Enemy enemy) { }
        public int AcceptAttack(IAttackVisitor visitor, Attributes stats) => visitor.VisitHeavy(this, stats);
        public int AcceptDefense(IDefenseVisitor visitor, Attributes stats) => visitor.VisitHeavy(this, stats);
    }
}
