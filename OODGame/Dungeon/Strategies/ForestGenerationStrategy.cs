using OODGame.Entities;
using OODGame.Items;
using OODGame.Items.Weapons;
using OODGame.Map;

namespace OODGame.Dungeon.Strategies
{
    public class ForestGenerationStrategy : IDungeonGenerationStrategy
    {
        private static readonly Random _rng = new Random();

        public Room Generate(DungeonBuilder builder, int mapX, int mapY,
                             Func<Enemy> enemyFactory, List<Item> items,
                             Item artifact, bool placeArtifact)
        {
            return builder
                .CreateEmpty()
                .AddScatteredTrees(_rng.Next(35, 55), clusterChance: 35)
                .AddItems(items, _rng.Next(8, 15))
                .AddWeapons(WeaponFactory.CreateRandom, _rng.Next(3, 6))
                .AddEnemies(enemyFactory, _rng.Next(2, 5))
                .AddArtifact(artifact, placeArtifact)
                .EnsurePassagesConnected(mapX, mapY, 3, 3)
                .Build();
        }
    }
}
