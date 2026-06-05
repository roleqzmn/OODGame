namespace OODGame.Startup
{
    public enum StartupMode
    {
        Server,
        Client
    }

    public sealed class StartupOptions
    {
        public const int DefaultPort = 5555;
        public const string DefaultIp = "127.0.0.1";

        public StartupMode Mode { get; }
        public string Ip { get; }
        public int Port { get; }

        public StartupOptions(StartupMode mode, string ip, int port)
        {
            Mode = mode;
            Ip = ip;
            Port = port;
        }
    }
}
