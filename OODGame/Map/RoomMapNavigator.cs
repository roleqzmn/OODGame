using System;
using System.Collections.Generic;

namespace OODGame.Map
{
    public sealed class RoomMapNavigator : IMapNavigator
    {
        private readonly Room _room;

        public RoomMapNavigator(Room room)
        {
            _room = room;
        }

        public bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < _room.Width && y >= 0 && y < _room.Height;
        }

        public bool IsWalkable(int x, int y)
        {
            if (!IsWithinBounds(x, y))
                return false;

            return _room.Grid[y, x].CanEnter();
        }

        public IReadOnlyList<(int x, int y)> GetWalkableNeighbors(int x, int y)
        {
            var neighbors = new List<(int x, int y)>(4);

            TryAddNeighbor(x + 1, y, neighbors);
            TryAddNeighbor(x - 1, y, neighbors);
            TryAddNeighbor(x, y + 1, neighbors);
            TryAddNeighbor(x, y - 1, neighbors);

            return neighbors;
        }

        public int? GetShortestPathDistance((int x, int y) from, (int x, int y) to, int maxDistance = int.MaxValue)
        {
            if (maxDistance < 0)
                return null;

            if (!IsWalkable(from.x, from.y) || !IsWalkable(to.x, to.y))
                return null;

            if (from == to)
                return 0;

            var visited = new bool[_room.Height, _room.Width];
            var queue = new Queue<(int x, int y, int distance)>();

            visited[from.y, from.x] = true;
            queue.Enqueue((from.x, from.y, 0));

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.distance >= maxDistance)
                    continue;

                foreach (var neighbor in GetWalkableNeighbors(current.x, current.y))
                {
                    if (visited[neighbor.y, neighbor.x])
                        continue;

                    int nextDistance = current.distance + 1;
                    if (neighbor == to)
                        return nextDistance;

                    visited[neighbor.y, neighbor.x] = true;
                    queue.Enqueue((neighbor.x, neighbor.y, nextDistance));
                }
            }

            return null;
        }

        private void TryAddNeighbor(int x, int y, List<(int x, int y)> neighbors)
        {
            if (IsWalkable(x, y))
                neighbors.Add((x, y));
        }
    }
}
