using System;
using System.Collections.Generic;
using System.Linq;
using OODGame.Entities;
using OODGame.Logger;
using OODGame.Players;

namespace OODGame.Map
{
    public static class EnemyMovementService
    {
        private static readonly Random _random = new Random();
        private const int SightRange = 8;

        public static HashSet<(int x, int y)> MoveEnemiesRandomly(Room room, int playerX, int playerY)
            => MoveEnemies(room, new[] { (playerX, playerY) });

        public static HashSet<(int x, int y)> MoveEnemies(Room room, IEnumerable<(int x, int y)> playerPositions)
            => MoveEnemies(room, playerPositions.Select(p => new Player(p.x, p.y, "remote")).ToList());

        public static HashSet<(int x, int y)> MoveEnemies(Room room, IReadOnlyCollection<Player> players)
        {
            var changedPositions = new HashSet<(int x, int y)>();
            var enemyPositions = new List<(int x, int y, EmptyTile tile)>();
            var playerPositions = players.Select(player => (player.Xpos, player.Ypos)).ToList();

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
                    .Where(p => !playerPositions.Any(player => player.Xpos == p.x && player.Ypos == p.y))
                    .ToList();

                var movingEnemy = currentTile.Enemy!;
                Player? adjacentPlayer = players.FirstOrDefault(player => Math.Abs(player.Xpos - entry.x) + Math.Abs(player.Ypos - entry.y) == 1);
                if (adjacentPlayer != null && movingEnemy.PlayerReaction == EnemyReactionMode.Follow)
                {
                    int damage = Math.Max(1, movingEnemy.Damage - 1);
                    adjacentPlayer.Stats.Health = Math.Max(0, adjacentPlayer.Stats.Health - damage);
                    EventLogger.Instance?.LogEvent($"{movingEnemy.Name} attacked {adjacentPlayer.Name} for {damage}.");
                    changedPositions.Add((entry.x, entry.y));
                    continue;
                }

                if (availableTargets.Count == 0)
                    continue;

                var target = ChooseTargetForEnemy(room, movingEnemy, entry.x, entry.y, availableTargets, players);
                if (target == null)
                    continue;

                if (room.Grid[target.Value.y, target.Value.x] is not EmptyTile targetTile)
                    continue;

                targetTile.SetEnemy(movingEnemy);
                currentTile.RemoveEnemy();
                movingEnemy.UpdatePosition(target.Value.x, target.Value.y);
                changedPositions.Add((entry.x, entry.y));
                changedPositions.Add((target.Value.x, target.Value.y));
            }

            return changedPositions;
        }

        private static (int x, int y)? ChooseTargetForEnemy(
            Room room,
            Enemy enemy,
            int enemyX,
            int enemyY,
            List<(int x, int y)> availableTargets,
            IReadOnlyCollection<Player> players)
        {
            Player? closestPlayer = FindNearestPlayer(room, enemyX, enemyY, players);
            if (closestPlayer != null && enemy.PlayerReaction != EnemyReactionMode.Ignore)
            {
                return enemy.PlayerReaction == EnemyReactionMode.Follow
                    ? SelectByPathDistance(room, enemyX, enemyY, availableTargets, (closestPlayer.Xpos, closestPlayer.Ypos), preferCloser: true)
                    : SelectByPathDistance(room, enemyX, enemyY, availableTargets, (closestPlayer.Xpos, closestPlayer.Ypos), preferCloser: false);
            }

            if (enemy.LastHeardSound.HasValue && enemy.SoundReaction != EnemyReactionMode.Ignore)
            {
                var source = enemy.LastHeardSound.Value;
                if (enemyX == source.x && enemyY == source.y)
                {
                    enemy.ClearLastHeardSound();
                }
                else
                {
                    return enemy.SoundReaction == EnemyReactionMode.Follow
                        ? SelectByPathDistance(room, enemyX, enemyY, availableTargets, source, preferCloser: true)
                        : SelectByPathDistance(room, enemyX, enemyY, availableTargets, source, preferCloser: false);
                }
            }

            return availableTargets[_random.Next(availableTargets.Count)];
        }

        private static Player? FindNearestPlayer(Room room, int enemyX, int enemyY, IReadOnlyCollection<Player> players)
        {
            Player? nearest = null;
            int bestDistance = int.MaxValue;

            foreach (var player in players)
            {
                int distance = room.Navigator.GetShortestPathDistance((enemyX, enemyY), (player.Xpos, player.Ypos), SightRange * 10)
                    ?? (Math.Abs(player.Xpos - enemyX) + Math.Abs(player.Ypos - enemyY));
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = player;
                }
            }

            return nearest;
        }

        private static (int x, int y)? SelectByPathDistance(
            Room room,
            int enemyX,
            int enemyY,
            List<(int x, int y)> availableTargets,
            (int x, int y) anchor,
            bool preferCloser)
        {
            if (availableTargets.Count == 0)
                return null;

            int bestScore = preferCloser ? int.MaxValue : int.MinValue;
            var bestTargets = new List<(int x, int y)>();

            foreach (var target in availableTargets)
            {
                int distance = room.Navigator.GetShortestPathDistance((target.x, target.y), anchor, SightRange * 10)
                    ?? (Math.Abs(target.x - anchor.x) + Math.Abs(target.y - anchor.y));
                if (preferCloser)
                {
                    if (distance < bestScore)
                    {
                        bestScore = distance;
                        bestTargets.Clear();
                        bestTargets.Add(target);
                    }
                    else if (distance == bestScore)
                    {
                        bestTargets.Add(target);
                    }
                }
                else
                {
                    if (distance > bestScore)
                    {
                        bestScore = distance;
                        bestTargets.Clear();
                        bestTargets.Add(target);
                    }
                    else if (distance == bestScore)
                    {
                        bestTargets.Add(target);
                    }
                }
            }

            if (bestTargets.Count == 0)
                return null;

            return bestTargets[_random.Next(bestTargets.Count)];
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
