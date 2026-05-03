using OODGame.Entities;
using OODGame.Items;
using OODGame.Map;

namespace OODGame.Dungeon
{
    public interface IDungeonGenerationStrategy
    {
        Room Generate(DungeonBuilder builder, int mapX, int mapY,
                      Func<Enemy> enemyFactory, List<Item> items,
                      Item artifact, bool placeArtifact);
    }
}
