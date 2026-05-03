using OODGame.Entities;
using OODGame.Items;
using OODGame.Items.Weapons;
using OODGame.Map;

namespace OODGame.Dungeon.Strategies
{
    public class HellGenerationStrategy : IDungeonGenerationStrategy
    {
        private static readonly Random _rng = new Random();

        public Room Generate(DungeonBuilder builder, int mapX, int mapY,
                             Func<Enemy> enemyFactory, List<Item> items,
                             Item artifact, bool placeArtifact)
        {
            return builder
                .CreateFilled()
                .AddCentralRoom(_rng.Next(8, 13), _rng.Next(6, 10))
                .AddPaths(_rng.Next(4, 7), _rng.Next(50, 90))
                .AddChambers(_rng.Next(5, 9), 3, 6)
                .AddItems(items, _rng.Next(2, 5))
                .AddWeapons(WeaponFactory.CreateRandom, _rng.Next(2, 4))
                .AddEnemies(enemyFactory, _rng.Next(5, 10))
                .AddArtifact(artifact, placeArtifact)
                .EnsurePassagesConnected(mapX, mapY, 3, 3)
                .Build();
        }
    }
}
