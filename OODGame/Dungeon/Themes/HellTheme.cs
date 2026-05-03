using OODGame.Dungeon.Strategies;
using OODGame.Entities;
using OODGame.Items;
using OODGame.Items.Artifacts;
using OODGame.Items.Unequipable;

namespace OODGame.Dungeon.Themes
{
    public class HellTheme : IDungeonTheme
    {
        private static readonly Random _rng = new Random();

        public string Name => "Hell";
        public string IntroMessage =>
            "The ground burns beneath your feet. Screams echo from the walls.\nYou have walked into Hell itself.";
        public IDungeonGenerationStrategy GenerationStrategy { get; } = new HellGenerationStrategy();
        public Func<Enemy> EnemyFactory => () => new Demon(
            health: _rng.Next(35, 60),
            damage: _rng.Next(8, 14),
            armor:  _rng.Next(3, 7));

        public List<Item> GetPossibleItems() => new List<Item>
        {
            new Unusable1(),
            new Unusable2(),
            new Unusable3(),
        };

        public Item CreateArtifact() => new HellstoneShard();
    }
}
