using OODGame.Fight.Visitors;
using OODGame.Logger;

namespace OODGame.Fight.Actions
{
    public class NormalFightAction : IFightAction
    {
        public string Name => "Normal Attack";

        public void Execute(FightContext ctx)
        {
            var visitor = new NormalAttackVisitor();
            int dealt = Math.Max(1, ctx.GetDamage(visitor) - ctx.Enemy.Armor);
            int taken  = Math.Max(1, ctx.Enemy.Damage - ctx.GetDefense(visitor));

            ctx.Enemy.Health -= dealt;
            ctx.Player.Stats.Health -= taken;
            ctx.LastLog = $"Normal attack: dealt {dealt}, took {taken}.";

            EventLogger.Instance?.LogEvent($"[Normal] {ctx.Player.Name} dealt {dealt} to {ctx.Enemy.Name}; took {taken}.");
        }
    }
}
