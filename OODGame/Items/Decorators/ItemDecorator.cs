using OODGame.Players;

namespace OODGame.Items.Decorators
{
    public abstract class ItemDecorator : Item
    {
        protected readonly Item _wrapped;

        protected ItemDecorator(Item wrapped)
        {
            _wrapped = wrapped;
            Name = _wrapped.Name;
            Description = _wrapped.Description;
            Symbol = _wrapped.Symbol;
            Weight = _wrapped.Weight;
        }

        public override bool CanEquip(Player player) => _wrapped.CanEquip(player);

        public override bool Equip(Player player)
        {
            ApplyEffect(player);
            return _wrapped.Equip(player);
        }

        public override void OnPickedUp(Player player) => _wrapped.OnPickedUp(player);

        protected abstract void ApplyEffect(Player player);
    }
}
