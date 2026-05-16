using System.Collections.Generic;

namespace OODGame.Map
{
    public interface IMapNavigator
    {
        bool IsWithinBounds(int x, int y);
        bool IsWalkable(int x, int y);
        IReadOnlyList<(int x, int y)> GetWalkableNeighbors(int x, int y);
        int? GetShortestPathDistance((int x, int y) from, (int x, int y) to, int maxDistance = int.MaxValue);
    }
}
