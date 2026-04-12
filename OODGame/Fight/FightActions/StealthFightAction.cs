using OODGame.Fight.Visitors;

namespace OODGame.Fight.Actions
{
    public class StealthFightAction : IFightAction
    {
        public string Name => "Stealth Attack";

        public void Execute(FightContext ctx)
        {
            var visitor = new StealthAttackVisitor();
            int dealt = Math.Max(0, ctx.GetDamage(visitor) - ctx.Enemy.Armor);
            int taken  = Math.Max(1, ctx.Enemy.Damage - ctx.GetDefense(visitor));

            ctx.Enemy.Health          -= dealt;
            ctx.Player.Stats.Health   -= taken;
            ctx.LastLog = $"Stealth attack: dealt {dealt}, took {taken}.";
        }
    }
}
