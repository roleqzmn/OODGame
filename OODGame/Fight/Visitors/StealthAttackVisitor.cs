using OODGame.Items.WeaponTypes;
using OODGame.Players;

namespace OODGame.Fight.Visitors
{
    // Stealth attack:  heavy=Damage/2, light=Damage*2, magical=1
    // Stealth defense: heavy=Str, light=Dex, magical=0, none=0
    public class StealthAttackVisitor : IAttackVisitor, IDefenseVisitor
    {
        public int VisitHeavy(HeavyWeapon weapon, Attributes stats)    => weapon.Damage / 2;
        public int VisitLight(LightWeapon weapon, Attributes stats)    => weapon.Damage * 2;
        public int VisitMagical(MagicalWeapon weapon, Attributes stats) => 1;
        public int VisitNoWeapon(Attributes stats)                      => 0;

        int IDefenseVisitor.VisitHeavy(HeavyWeapon weapon, Attributes stats)    => stats.Strength;
        int IDefenseVisitor.VisitLight(LightWeapon weapon, Attributes stats)    => stats.Dexterity;
        int IDefenseVisitor.VisitMagical(MagicalWeapon weapon, Attributes stats) => 0;
        int IDefenseVisitor.VisitNoWeapon(Attributes stats)                      => 0;
    }
}
