using System;
using System.Collections.Generic;
using System.Linq;

namespace OODGame.Map
{
    public static class EnemyMovementService
    {
        private static readonly Random _random = new Random();

        public static HashSet<(int x, int y)> MoveEnemiesRandomly(Room room, int playerX, int playerY)
        {
            var changedPositions = new HashSet<(int x, int y)>();
            var enemyPositions = new List<(int x, int y, EmptyTile tile)>();

            for (int y = 0; y < room.Height; y++)
            {
                for (int x = 0; x < room.Width; x++)
                {
                    if (room.Grid[y, x] is EmptyTile emptyTile && emptyTile.HasEnemy)
                        enemyPositions.Add((x, y, emptyTile));
                }
            }

            Shuffle(enemyPositions);

            foreach (var entry in enemyPositions)
            {
                if (room.Grid[entry.y, entry.x] is not EmptyTile currentTile || !currentTile.HasEnemy)
                    continue;

                if (!ReferenceEquals(currentTile.Enemy, entry.tile.Enemy))
                    continue;

                var availableTargets = room.Navigator
                    .GetWalkableNeighbors(entry.x, entry.y)
                    .Where(p => room.Grid[p.y, p.x] is EmptyTile targetTile && !targetTile.HasEnemy)
                    .Where(p => !(p.x == playerX && p.y == playerY))
                    .ToList();

                if (availableTargets.Count == 0)
                    continue;

                var target = availableTargets[_random.Next(availableTargets.Count)];

                if (room.Grid[target.y, target.x] is not EmptyTile targetTile)
                    continue;

                var movingEnemy = currentTile.Enemy!;
                targetTile.SetEnemy(movingEnemy);
                currentTile.RemoveEnemy();
                movingEnemy.UpdatePosition(target.x, target.y);
                changedPositions.Add((entry.x, entry.y));
                changedPositions.Add((target.x, target.y));
            }

            return changedPositions;
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
