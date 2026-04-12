using OODGame.Players;

namespace OODGame.Items.Decorators
{
    public class ProtectiveDecorator : WeaponDecorator
    {
        public ProtectiveDecorator(Weapon wrapped) : base(wrapped)
        {
            Name = _wrapped.Name + " (Protective)";
        }

        protected override void ApplyEffect(Player player) => player.Stats.Dexterity += 5;
    }
}
