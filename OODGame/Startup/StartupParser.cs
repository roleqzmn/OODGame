using System;

namespace OODGame.Startup
{
    public static class StartupParser
    {
        public static bool TryParse(string[] args, out StartupOptions? options, out string? error)
        {
            options = null;
            error = null;

            if (args.Length == 0)
            {
                error = "No arguments provided.";
                return false;
            }

            string mode = args[0].Trim().ToLowerInvariant();
            switch (mode)
            {
                case "--server":
                    return TryParseServer(args, out options, out error);
                case "--client":
                    return TryParseClient(args, out options, out error);
                default:
                    error = $"Unknown startup mode: {args[0]}";
                    return false;
            }
        }

        public static bool TryParsePort(string? rawPort, out int port)
        {
            port = StartupOptions.DefaultPort;
            if (string.IsNullOrWhiteSpace(rawPort))
                return true;

            if (!int.TryParse(rawPort, out port))
                return false;

            return port is > 0 and <= 65535;
        }

        public static bool TryParseEndpoint(string? rawEndpoint, out string ip, out int port)
        {
            ip = StartupOptions.DefaultIp;
            port = StartupOptions.DefaultPort;

            if (string.IsNullOrWhiteSpace(rawEndpoint))
                return true;

            string[] parts = rawEndpoint.Split(':', StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
                return false;

            if (string.IsNullOrWhiteSpace(parts[0]))
                return false;

            if (!TryParsePort(parts[1], out port))
                return false;

            ip = parts[0];
            return true;
        }

        private static bool TryParseServer(string[] args, out StartupOptions? options, out string? error)
        {
            options = null;
            error = null;

            if (args.Length > 2)
            {
                error = "Too many arguments for server mode.";
                return false;
            }

            string? rawPort = args.Length == 2 ? args[1] : null;
            if (!TryParsePort(rawPort, out int port))
            {
                error = "Invalid server port. Expected 1-65535.";
                return false;
            }

            options = new StartupOptions(StartupMode.Server, StartupOptions.DefaultIp, port);
            return true;
        }

        private static bool TryParseClient(string[] args, out StartupOptions? options, out string? error)
        {
            options = null;
            error = null;

            if (args.Length > 2)
            {
                error = "Too many arguments for client mode.";
                return false;
            }

            string? rawEndpoint = args.Length == 2 ? args[1] : null;
            if (!TryParseEndpoint(rawEndpoint, out string ip, out int port))
            {
                error = "Invalid client endpoint. Expected ip:port.";
                return false;
            }

            options = new StartupOptions(StartupMode.Client, ip, port);
            return true;
        }
    }
}
