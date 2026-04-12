using OODGame.Players;

namespace OODGame.Fight
{
    public interface IWeaponCategory
    {
        int AcceptAttack(IAttackVisitor visitor, Attributes stats);
        int AcceptDefense(IDefenseVisitor visitor, Attributes stats);
    }
}
