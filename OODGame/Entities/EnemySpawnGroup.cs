using System;

namespace OODGame.Entities
{
    public sealed class EnemySpawnGroup
    {
        public Func<Enemy> Factory { get; }
        public int Count { get; }

        public EnemySpawnGroup(Func<Enemy> factory, int count)
        {
            Factory = factory;
            Count = count;
        }
    }
}
