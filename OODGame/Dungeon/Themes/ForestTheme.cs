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
        public Func<Enemy> EnemyFactory => () => new Wolf(
            health: _rng.Next(20, 35),
            damage: _rng.Next(5, 10),
            armor:  _rng.Next(1, 4));

        public List<Item> GetPossibleItems() => new List<Item>
        {
            new Unusable1(),
            new Unusable2(),
            new Unusable3(),
        };

        public Item CreateArtifact() => new AncientSeed();
    }
}
