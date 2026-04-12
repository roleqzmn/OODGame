using System;

namespace OODGame.Entities
{
    public static class EnemyFactory
    {
        private static readonly Random _random = new Random();

        public static Enemy CreateRandom() => CreateRandomGoblin();

        public static Goblin CreateRandomGoblin() => new Goblin(
            health: _random.Next(20, 45),
            damage: _random.Next(3,  9),
            armor:  _random.Next(1,  5)
        );
    }
}
