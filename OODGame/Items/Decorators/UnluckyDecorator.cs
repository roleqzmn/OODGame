using OODGame.Players;

namespace OODGame.Items.Decorators
{
    public class UnluckyDecorator : WeaponDecorator
    {
        public UnluckyDecorator(Weapon wrapped) : base(wrapped)
        {
            Name = _wrapped.Name + " (Unlucky)";
        }

        protected override void ApplyEffect(Player player) => player.Stats.Luck -= 5;
    }
}
