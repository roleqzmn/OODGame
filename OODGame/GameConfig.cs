
using System.Text.Json;
using System.Text.Json.Serialization;
using OODGame.Dungeon;

namespace OODGame
{
    public sealed class GameConfig
    {
        public string PlayerName { get; set; } = "roleq";
        public string LogPath { get; set; } = ".\\logs.txt";
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DungeonThemeType DungeonTheme { get; set; } = DungeonThemeType.Random;

        public static GameConfig? Instance { get; private set; }

        [JsonConstructor]
        private GameConfig() { }

        public static GameConfig Load(string path = ".\\GameConfig.json")
        {
            if (Instance != null)
                return Instance;

            if (!File.Exists(path))
            {
                Instance = new GameConfig();
                return Instance;
            }

            string json = File.ReadAllText(path);
            Instance = JsonSerializer.Deserialize<GameConfig>(json)
                ?? throw new InvalidDataException("Failed to load configuration.");
            return Instance;
        }
    }
}

