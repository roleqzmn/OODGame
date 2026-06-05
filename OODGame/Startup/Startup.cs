using System;

namespace OODGame.Startup
{
    public static class Startup
    {
        public static StartupOptions Resolve(string[] args)
        {
            if (args.Length == 0)
            {
                return StartupPrompt.Prompt();
            }

            if (StartupParser.TryParse(args, out var options, out var error))
            {
                return options!;
            }

            Console.WriteLine(error);
            Console.WriteLine(GetUsage());
            return StartupPrompt.Prompt();
        }

        public static string GetUsage()
        {
            return "Usage: --server [port] | --client [ip:port]";
        }
    }
}
