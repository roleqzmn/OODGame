using OODGame.Dungeon.Strategies;
using OODGame.Entities;
using OODGame.Items;
using OODGame.Items.Artifacts;
using OODGame.Items.Unequipable;

namespace OODGame.Dungeon.Themes
{
    public class ForestTheme : IDungeonTheme
    {
        private static readonly Random _rng = new Random();

        public string Name => "Forest";
        public string IntroMessage =>
            "You step into a cursed forest. The trees watch your every move.\nNot all that hunts here walks on two legs.";
        public IDungeonGenerationStrategy GenerationStrategy { get; } = new ForestGenerationStrategy();
        public IReadOnlyList<EnemySpawnGroup> EnemyGroups => new List<EnemySpawnGroup>
        {
            new EnemySpawnGroup(
                factory: () => new Wolf(
                    health: _rng.Next(20, 35),
                    damage: _rng.Next(5, 10),
                    armor:  _rng.Next(1, 4)),
                count: _rng.Next(2, 5))
        };

        public List<Item> GetPossibleItems() => new List<Item>
        {
            new Unusable1(),
            new Unusable2(),
            new Unusable3(),
        };

        public Item CreateArtifact() => new AncientSeed();
    }
}
