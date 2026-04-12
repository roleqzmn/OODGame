using System;
namespace OODGame.Fight.Actions
{
    public interface IFightAction
    {
        string Name { get; }
        void Execute(FightContext ctx);
    }
}

