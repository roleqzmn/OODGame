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
        IReadOnlyList<EnemySpawnGroup> EnemyGroups { get; }
        List<Item> GetPossibleItems();
        Item CreateArtifact();
    }
}
