using OODGame.Entities;
using OODGame.Items;
using OODGame.Items.Weapons;
using OODGame.Map;

namespace OODGame.Dungeon.Strategies
{
    public class CryptGenerationStrategy : IDungeonGenerationStrategy
    {
        private static readonly Random _rng = new Random();

        public Room Generate(DungeonBuilder builder, int mapX, int mapY,
                             Func<Enemy> enemyFactory, List<Item> items,
                             Item artifact, bool placeArtifact)
        {
            return builder
                .CreateFilled()
                .AddCentralRoom(_rng.Next(6, 10), _rng.Next(5, 8))
                .AddPaths(_rng.Next(6, 10), _rng.Next(60, 100))
                .AddChambers(_rng.Next(8, 14), 2, 4)
                .AddItems(items, _rng.Next(4, 8))
                .AddWeapons(WeaponFactory.CreateRandom, _rng.Next(2, 5))
                .AddEnemies(enemyFactory, _rng.Next(4, 8))
                .AddArtifact(artifact, placeArtifact)
                .EnsurePassagesConnected(mapX, mapY, 3, 3)
                .Build();
        }
    }
}
