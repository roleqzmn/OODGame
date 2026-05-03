using OODGame.Fight.Visitors;
using OODGame.Logger;

namespace OODGame.Fight.Actions
{
    public class MagicalFightAction : IFightAction
    {
        public string Name => "Magical Attack";

        public void Execute(FightContext ctx)
        {
            var visitor = new MagicalAttackVisitor();
            int dealt = Math.Max(1, ctx.GetDamage(visitor) - ctx.Enemy.Armor);
            int taken  = Math.Max(1, ctx.Enemy.Damage - ctx.GetDefense(visitor));

            ctx.Enemy.Health -= dealt;
            ctx.Player.Stats.Health -= taken;
            ctx.LastLog = $"Magical attack: dealt {dealt}, took {taken}.";

            EventLogger.Instance?.LogEvent($"[Magical] {ctx.Player.Name} dealt {dealt} to {ctx.Enemy.Name}; took {taken}.");
        }
    }
}
