using OODGame.Players;
using OODGame.Entities;
using OODGame.Fight;

namespace OODGame.Items.Decorators
{
    public abstract class WeaponDecorator : Weapon, IWeaponCategory
    {
        protected readonly Weapon _wrapped;

        protected WeaponDecorator(Weapon wrapped)
        {
            _wrapped = wrapped;
            Name = _wrapped.Name;
            Description = _wrapped.Description;
            Symbol = _wrapped.Symbol;
            Weight = _wrapped.Weight;
            Damage = _wrapped.Damage;
            MinLvl = _wrapped.MinLvl;
            Range = _wrapped.Range;
        }

        public override bool IsTwoHanded => _wrapped.IsTwoHanded;
        public override bool CanEquip(Player player) => _wrapped.CanEquip(player);
        public override void OnPickedUp(Player player) => _wrapped.OnPickedUp(player);
        public override void Attack(Player player, Enemy enemy) => _wrapped.Attack(player, enemy);

        public override bool Equip(Player player)
        {
            ApplyEffect(player);
            return player.EquipWeapon(this);
        }

        public int AcceptAttack(IAttackVisitor visitor, Attributes stats)
            => (_wrapped as IWeaponCategory)?.AcceptAttack(visitor, stats) ?? visitor.VisitNoWeapon(stats);

        public int AcceptDefense(IDefenseVisitor visitor, Attributes stats)
            => (_wrapped as IWeaponCategory)?.AcceptDefense(visitor, stats) ?? visitor.VisitNoWeapon(stats);

        protected abstract void ApplyEffect(Player player);
    }
}

