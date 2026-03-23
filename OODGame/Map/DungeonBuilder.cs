using System;
using System;
using System.Collections.Generic;
using OODGame.Items;

namespace OODGame.Map
{
    public class DungeonBuilder
    {
        private Tile[,]? _grid;
        private int _width;
        private int _height;
        private Random _random = new Random();
        private bool _initialized = false;
        public List<(int x, int y)> Passages { get; private set; } = new List<(int, int)>();

        public DungeonBuilder(int width, int height)
        {
            _width = width;
            _height = height;
        }
        public DungeonBuilder CreateEmpty()
        {
            _grid = new Tile[_height, _width];
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _grid[y, x] = new EmptyTile();
                }
            }
            _initialized = true;
            return this;
        }
        public DungeonBuilder CreateFilled()
        {
            _grid = new Tile[_height, _width];
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _grid[y, x] = new WallTile();
                }
            }
            _initialized = true;
            return this;
        }
    #pragma warning disable CS8602
        public DungeonBuilder AddPaths(int pathCount = 5, int pathLength = 100)
        {
            ThrowIfNotInitialized();
            List<(int x, int y)> cells = new List<(int, int)>();
            int cellSize = 5;

            for (int y = cellSize; y < _height - cellSize; y += cellSize)
            {
                for (int x = cellSize; x < _width - cellSize; x += cellSize)
                {
                    cells.Add((x, y));
                }
            }

            if (cells.Count == 0)
                return this;
            HashSet<(int, int)> connected = new HashSet<(int, int)>();
            List<(int, int)> toConnect = new List<(int, int)>(cells);
            
            connected.Add(toConnect[0]);
            toConnect.RemoveAt(0);

            while (toConnect.Count > 0)
            {
                int nearestIdx = 0;
                int minDist = int.MaxValue;

                for (int i = 0; i < toConnect.Count; i++)
                {
                    int minDistToConnected = int.MaxValue;
                    foreach (var conn in connected)
                    {
                        int dist = Math.Abs(toConnect[i].Item1 - conn.Item1) + Math.Abs(toConnect[i].Item2 - conn.Item2);
                        minDistToConnected = Math.Min(minDistToConnected, dist);
                    }

                    if (minDistToConnected < minDist)
                    {
                        minDist = minDistToConnected;
                        nearestIdx = i;
                    }
                }
                var nearest = toConnect[nearestIdx];
                toConnect.RemoveAt(nearestIdx);

                var closestConnected = connected.First();
                int closestDist = Math.Abs(nearest.Item1 - closestConnected.Item1) + Math.Abs(nearest.Item2 - closestConnected.Item2);

                foreach (var conn in connected)
                {
                    int dist = Math.Abs(nearest.Item1 - conn.Item1) + Math.Abs(nearest.Item2 - conn.Item2);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestConnected = conn;
                    }
                }

                CarvePathBetween(closestConnected, nearest);
                connected.Add(nearest);
            }

            return this;
        }

        private void CarvePathBetween((int x, int y) from, (int x, int y) to)
        {
            ThrowIfNotInitialized();
            int x = from.x;
            int y = from.y;

            if (_random.Next(2) == 0)
            {
                while (x != to.x)
                {
                    _grid[y, x] = new EmptyTile();
                    x += (x < to.x) ? 1 : -1;
                }

                while (y != to.y)
                {
                    _grid[y, x] = new EmptyTile();
                    y += (y < to.y) ? 1 : -1;
                }
            }
            else
            {
                while (y != to.y)
                {
                    _grid[y, x] = new EmptyTile();
                    y += (y < to.y) ? 1 : -1;
                }

                while (x != to.x)
                {
                    _grid[y, x] = new EmptyTile();
                    x += (x < to.x) ? 1 : -1;
                }
            }

            _grid[to.y, to.x] = new EmptyTile();
        }
        public DungeonBuilder AddChambers(int chamberCount = 10, int minSize = 3, int maxSize = 8)
        {
            ThrowIfNotInitialized();
            int additionalChambers = 4;
            
            for (int i = 0; i < additionalChambers; i++)
            {
                int chamberWidth = _random.Next(4, 7);
                int chamberHeight = _random.Next(3, 6);
                
                int startX = _random.Next(2, _width - chamberWidth - 2);
                int startY = _random.Next(2, _height - chamberHeight - 2);

                for (int y = startY; y < startY + chamberHeight; y++)
                {
                    for (int x = startX; x < startX + chamberWidth; x++)
                    {
                        if (x >= 0 && x < _width && y >= 0 && y < _height)
                        {
                            _grid[y, x] = new EmptyTile();
                        }
                    }
                }

                ConnectChamberToNetwork(startX + chamberWidth / 2, startY + chamberHeight / 2);
            }

            return this;
        }

        private void ConnectChamberToNetwork(int chamberX, int chamberY)
        {
            int closestDist = int.MaxValue;
            int closestX = chamberX;
            int closestY = chamberY;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_grid[y, x] is EmptyTile)
                    {
                        int dist = Math.Abs(x - chamberX) + Math.Abs(y - chamberY);
                        if (dist < closestDist && dist > 1) 
                        {
                            closestDist = dist;
                            closestX = x;
                            closestY = y;
                        }
                    }
                }
            }
            CarvePathBetween((chamberX, chamberY), (closestX, closestY));
        }
        public DungeonBuilder AddCentralRoom(int width, int height)
        {
            ThrowIfNotInitialized();

            int startX = (_width - width) / 2;
            int startY = (_height - height) / 2;

            for (int y = startY; y < startY + height; y++)
            {
                for (int x = startX; x < startX + width; x++)
                {
                    if (x >= 0 && x < _width && y >= 0 && y < _height)
                    {
                        _grid[y, x] = new EmptyTile();
                    }
                }
            }
            return this;
        }
        public DungeonBuilder AddItems(List<Item> items, int itemCount)
        {
            ThrowIfNotInitialized();

            if (items == null || items.Count == 0)
                return this;

            int placedItems = 0;
            int maxAttempts = itemCount * 10;
            int attempts = 0;

            while (placedItems < itemCount && attempts < maxAttempts)
            {
                int x = _random.Next(_width);
                int y = _random.Next(_height);

                if (_grid[y, x] is EmptyTile emptyTile)
                {
                    var item = items[_random.Next(items.Count)];
                    emptyTile.PlaceItem(item);
                    placedItems++;
                }

                attempts++;
            }
            return this;
        }
        public DungeonBuilder AddWeapons(List<Weapon> weapons, int weaponCount)
        {
            ThrowIfNotInitialized();

            if (weapons == null || weapons.Count == 0)
                return this;

            int placedWeapons = 0;
            int maxAttempts = weaponCount * 10;
            int attempts = 0;

            while (placedWeapons < weaponCount && attempts < maxAttempts)
            {
                int x = _random.Next(_width);
                int y = _random.Next(_height);

                if (_grid[y, x] is EmptyTile emptyTile)
                {
                    var weapon = weapons[_random.Next(weapons.Count)];
                    emptyTile.PlaceItem(weapon);
                    placedWeapons++;
                }

                attempts++;
            }
            return this;
        }

        public Room Build()
        {
            ThrowIfNotInitialized();

            if (_grid == null)
                throw new InvalidOperationException();

            FindPassages();
            return new Room(_width, _height, _grid);
        }

        public DungeonBuilder EnsurePassagesConnected(int mapX, int mapY, int mapWidth = 3, int mapHeight = 3)
        {
            ThrowIfNotInitialized();

            if (mapY > 0)
            {
                int passageX = _width / 2;
                for (int x = passageX - 2; x <= passageX + 2; x++)
                {
                    if (x >= 0 && x < _width)
                        _grid[0, x] = new EmptyTile();
                }
                for (int y = 0; y < _height / 2; y++)
                {
                    _grid[y, passageX] = new EmptyTile();
                }
            }
            if (mapY < mapHeight - 1)
            {
                int passageX = _width / 2;
                for (int x = passageX - 2; x <= passageX + 2; x++)
                {
                    if (x >= 0 && x < _width)
                        _grid[_height - 1, x] = new EmptyTile();
                }
                for (int y = _height - 1; y >= _height / 2; y--)
                {
                    _grid[y, passageX] = new EmptyTile();
                }
            }
            if (mapX > 0)
            {
                int passageY = _height / 2;
                for (int y = passageY - 2; y <= passageY + 2; y++)
                {
                    if (y >= 0 && y < _height)
                        _grid[y, 0] = new EmptyTile();
                }
                for (int x = 0; x < _width / 2; x++)
                {
                    _grid[passageY, x] = new EmptyTile();
                }
            }
            if (mapX < mapWidth - 1)
            {
                int passageY = _height / 2;
                for (int y = passageY - 2; y <= passageY + 2; y++)
                {
                    if (y >= 0 && y < _height)
                        _grid[y, _width - 1] = new EmptyTile();
                }
                for (int x = _width - 1; x >= _width / 2; x--)
                {
                    _grid[passageY, x] = new EmptyTile();
                }
            }
            return this;
        }

        private void FindPassages()
        {
            Passages.Clear();

            for (int x = 0; x < _width; x++)
            {
                if (_grid![0, x] is EmptyTile)
                    Passages.Add((x, 0));
            }
            for (int x = 0; x < _width; x++)
            {
                if (_grid[_height - 1, x] is EmptyTile)
                    Passages.Add((x, _height - 1));
            }
            for (int y = 1; y < _height - 1; y++)
            {
                if (_grid[y, 0] is EmptyTile)
                    Passages.Add((0, y));
            }
            for (int y = 1; y < _height - 1; y++)
            {
                if (_grid[y, _width - 1] is EmptyTile)
                    Passages.Add((_width - 1, y));
            }
        }
        #pragma warning restore CS8602
        private void ThrowIfNotInitialized()
        {
            if (!_initialized || _grid == null)
                throw new InvalidOperationException();
        }
    }
}
