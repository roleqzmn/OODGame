using OODGame.Players;

namespace OODGame.Items.Decorators
{
    public class LuckyDecorator : WeaponDecorator
    {
        public LuckyDecorator(Weapon wrapped) : base(wrapped)
        {
            Name = _wrapped.Name + " (Lucky)";
        }

        protected override void ApplyEffect(Player player) => player.Stats.Luck += 5;
    }
}
