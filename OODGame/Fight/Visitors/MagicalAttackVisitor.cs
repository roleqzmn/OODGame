using OODGame.Items.WeaponTypes;
using OODGame.Players;

namespace OODGame.Fight.Visitors
{
    // Magical attack:  magical=Damage, heavy=1, light=1
    // Magical defense: heavy=Luck, light=Luck, magical=Wisdom*2, none=Luck
    public class MagicalAttackVisitor : IAttackVisitor, IDefenseVisitor
    {
        public int VisitHeavy(HeavyWeapon weapon, Attributes stats)    => 1;
        public int VisitLight(LightWeapon weapon, Attributes stats)    => 1;
        public int VisitMagical(MagicalWeapon weapon, Attributes stats) => weapon.Damage;
        public int VisitNoWeapon(Attributes stats)                      => 0;

        int IDefenseVisitor.VisitHeavy(HeavyWeapon weapon, Attributes stats)    => stats.Luck;
        int IDefenseVisitor.VisitLight(LightWeapon weapon, Attributes stats)    => stats.Luck;
        int IDefenseVisitor.VisitMagical(MagicalWeapon weapon, Attributes stats) => stats.Wisdom * 2;
        int IDefenseVisitor.VisitNoWeapon(Attributes stats)                      => stats.Luck;
    }
}
