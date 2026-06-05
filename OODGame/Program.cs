using OODGame;
using OODGame.Startup;
using System.Text;

namespace ODDGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            StartupOptions startupOptions = Startup.Resolve(args);
            switch (startupOptions.Mode)
            {
                case StartupMode.Server:
                    Console.WriteLine($"Starting in SERVER mode on port {startupOptions.Port}.");
                    break;
                case StartupMode.Client:
                    Console.WriteLine($"Starting in CLIENT mode for {startupOptions.Ip}:{startupOptions.Port}.");
                    break;
            }

            GameConfig config = GameConfig.Load("GameConfig.json");
            Game game = new Game(config);
            game.Run();
        }
    }
}