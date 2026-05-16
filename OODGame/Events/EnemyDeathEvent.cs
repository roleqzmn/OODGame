using System;

namespace OODGame.Events
{
    public sealed class EnemyDeathEvent : IGameEvent
    {
        public Guid EnemyId { get; }
        public string Species { get; }

        public EnemyDeathEvent(Guid enemyId, string species)
        {
            EnemyId = enemyId;
            Species = species;
        }
    }
}
