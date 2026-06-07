using OODGame;
using OODGame.Networking.Client;
using OODGame.Networking.Server;
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
            GameConfig config = GameConfig.Load("GameConfig.json");

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
            };

            switch (startupOptions.Mode)
            {
                case StartupMode.Server:
                    Console.WriteLine($"Starting in SERVER mode on port {startupOptions.Port}.");
                    var server = new ServerRuntime(startupOptions, config);
                    server.RunAsync(cts.Token).GetAwaiter().GetResult();
                    break;
                case StartupMode.Client:
                    Console.WriteLine($"Starting in CLIENT mode for {startupOptions.Ip}:{startupOptions.Port}.");
                    var client = new ClientRuntime(startupOptions);
                    client.RunAsync(cts.Token).GetAwaiter().GetResult();
                    break;
            }
        }
    }
}