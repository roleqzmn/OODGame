using System.Text.Json;

namespace OODGame
{
    public sealed class GameConfig
    {
        public string PlayerName { get; set; } = "roleq";
        public string LogPath { get; set; } = "logs.txt";

        public static GameConfig? Instance { get; private set; }

        private GameConfig() { }

        public static GameConfig Load(string path = "GameConfig.json")
        {
            if (Instance != null)
                return Instance;

            if (!File.Exists(path))
                throw new FileNotFoundException($"No config file found: {path}");

            string json = File.ReadAllText(path);
            Instance = JsonSerializer.Deserialize<GameConfig>(json)
                ?? throw new InvalidDataException("Failed to load configuration.");
            return Instance;
        }
    }
}
