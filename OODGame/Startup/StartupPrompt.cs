using System;

namespace OODGame.Startup
{
    public static class StartupPrompt
    {
        public static StartupOptions Prompt()
        {
            while (true)
            {
                Console.Write("Start as (S)erver or (C)lient? ");
                ConsoleKey key = Console.ReadKey(intercept: true).Key;
                Console.WriteLine(char.ToUpperInvariant((char)key));

                if (key == ConsoleKey.S)
                {
                    int port = PromptForPort();
                    return new StartupOptions(StartupMode.Server, StartupOptions.DefaultIp, port);
                }

                if (key == ConsoleKey.C)
                {
                    (string ip, int port) = PromptForEndpoint();
                    return new StartupOptions(StartupMode.Client, ip, port);
                }

                Console.WriteLine("Please choose S or C.");
            }
        }

        private static int PromptForPort()
        {
            while (true)
            {
                Console.Write($"Port [{StartupOptions.DefaultPort}]: ");
                string? raw = Console.ReadLine();
                if (StartupParser.TryParsePort(raw, out int port))
                    return port;

                Console.WriteLine("Invalid port. Use value in range 1-65535.");
            }
        }

        private static (string ip, int port) PromptForEndpoint()
        {
            while (true)
            {
                Console.Write($"Server endpoint [{StartupOptions.DefaultIp}:{StartupOptions.DefaultPort}]: ");
                string? raw = Console.ReadLine();
                if (StartupParser.TryParseEndpoint(raw, out string ip, out int port))
                    return (ip, port);

                Console.WriteLine("Invalid endpoint. Use format ip:port.");
            }
        }
    }
}
