using OODGame.Dungeon.Themes;

namespace OODGame.Dungeon
{
    public enum DungeonThemeType
    {
        Random,
        Crypt,
        Forest,
        Hell
    }

    public static class DungeonThemeFactory
    {
        private static readonly Random _rng = new Random();

        public static IDungeonTheme Create(DungeonThemeType type) => type switch
        {
            DungeonThemeType.Crypt   => new CryptTheme(),
            DungeonThemeType.Forest  => new ForestTheme(),
            DungeonThemeType.Hell    => new HellTheme(),
            DungeonThemeType.Random  => CreateRandom(),
            _                        => CreateRandom()
        };

        private static IDungeonTheme CreateRandom() =>
            _rng.Next(3) switch
            {
                0 => new CryptTheme(),
                1 => new ForestTheme(),
                _ => new HellTheme()
            };
    }
}
