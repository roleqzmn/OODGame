using OODGame.Items.WeaponTypes;
using OODGame.Players;

namespace OODGame.Fight
{
    public interface IDefenseVisitor
    {
        int VisitHeavy(HeavyWeapon weapon, Attributes stats);
        int VisitLight(LightWeapon weapon, Attributes stats);
        int VisitMagical(MagicalWeapon weapon, Attributes stats);
        int VisitNoWeapon(Attributes stats);
    }
}
