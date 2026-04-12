using OODGame.Players;

namespace OODGame.Items.Decorators
{
    /// <summary>
    /// Efekt: +5 do Damage broni. Zachowuje kategoriê (Heavy/Light/Magical).
    /// </summary>
    public class StrongDecorator : WeaponDecorator
    {
        public StrongDecorator(Weapon wrapped) : base(wrapped)
        {
            Name = _wrapped.Name + " (Strong)";
            Damage = (Int16)(_wrapped.Damage + 5);
        }

        protected override void ApplyEffect(Player player) { }
    }
}
