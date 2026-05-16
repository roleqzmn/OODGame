using OODGame.Dungeon.Strategies;
using OODGame.Entities;
using OODGame.Items;
using OODGame.Items.Artifacts;
using OODGame.Items.Unequipable;

namespace OODGame.Dungeon.Themes
{
    public class CryptTheme : IDungeonTheme
    {
        private static readonly Random _rng = new Random();

        public string Name => "Crypt";
        public string IntroMessage =>
            "You descend into an ancient crypt. The air reeks of decay.\nThe dead do not rest here.";
        public IDungeonGenerationStrategy GenerationStrategy { get; } = new CryptGenerationStrategy();
        public IReadOnlyList<EnemySpawnGroup> EnemyGroups => new List<EnemySpawnGroup>
        {
            new EnemySpawnGroup(
                factory: () => new Skeleton(
                    health: _rng.Next(15, 30),
                    damage: _rng.Next(6, 12),
                    armor:  _rng.Next(0, 3)),
                count: _rng.Next(4, 8))
        };

        public List<Item> GetPossibleItems() => new List<Item>
        {
            new Unusable1(),
            new Unusable2(),
            new Unusable3(),
        };

        public Item CreateArtifact() => new CursedSkull();
    }
}
