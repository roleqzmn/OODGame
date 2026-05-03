using OODGame.Entities;
using OODGame.Items;
using OODGame.Map;

namespace OODGame.Dungeon
{
    public interface IDungeonTheme
    {
        string Name { get; }
        string IntroMessage { get; }
        IDungeonGenerationStrategy GenerationStrategy { get; }
        Func<Enemy> EnemyFactory { get; }
        List<Item> GetPossibleItems();
        Item CreateArtifact();
    }
}
