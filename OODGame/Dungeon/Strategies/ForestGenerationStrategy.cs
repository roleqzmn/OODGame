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
                .CreateFilled()
                .AddCentralRoom(_rng.Next(16, 22), _rng.Next(12, 16))
                .AddPaths(_rng.Next(3, 5), _rng.Next(40, 70))
                .AddChambers(_rng.Next(3, 6), 4, 8)
                .AddItems(items, _rng.Next(8, 15))
                .AddWeapons(WeaponFactory.CreateRandom, _rng.Next(3, 6))
                .AddEnemies(enemyFactory, _rng.Next(2, 5))
                .AddArtifact(artifact, placeArtifact)
                .EnsurePassagesConnected(mapX, mapY, 3, 3)
                .Build();
        }
    }
}
