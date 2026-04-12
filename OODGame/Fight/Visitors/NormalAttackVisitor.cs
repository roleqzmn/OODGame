using OODGame.Items.WeaponTypes;
using OODGame.Players;

namespace OODGame.Fight.Visitors
{
    // Normal attack:  heavy=Damage, light=Damage, magical=1
    // Normal defense: heavy=Str+Luck, light=Dex+Luck, magical=Dex+Luck, none=Dex
    public class NormalAttackVisitor : IAttackVisitor, IDefenseVisitor
    {
        public int VisitHeavy(HeavyWeapon weapon, Attributes stats)    => weapon.Damage;
        public int VisitLight(LightWeapon weapon, Attributes stats)    => weapon.Damage;
        public int VisitMagical(MagicalWeapon weapon, Attributes stats) => 1;
        public int VisitNoWeapon(Attributes stats)                      => 0;

        int IDefenseVisitor.VisitHeavy(HeavyWeapon weapon, Attributes stats)    => stats.Strength + stats.Luck;
        int IDefenseVisitor.VisitLight(LightWeapon weapon, Attributes stats)    => stats.Dexterity + stats.Luck;
        int IDefenseVisitor.VisitMagical(MagicalWeapon weapon, Attributes stats) => stats.Dexterity + stats.Luck;
        int IDefenseVisitor.VisitNoWeapon(Attributes stats)                      => stats.Dexterity;
    }
}
