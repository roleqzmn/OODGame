using OODGame.Items;
using System;
using System.Collections.Generic;

namespace OODGame.Logger
{
    public sealed class EventLogger : IEventLogger<string>
    {
        public List<string> RecentEvents { get; private set; }
        public int MaxEvents { get; private set; } = 8;
        private readonly string _path;
        private readonly FileStream _logFile;
        public static EventLogger? Instance { get; private set; }

        private EventLogger(string path)
        {
            RecentEvents = new List<string>();
            _path = path;


            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            _logFile = new FileStream(_path, FileMode.Append, FileAccess.Write);
        }

        public static EventLogger GetInstance(string path)
        {
            if (Instance == null)
                Instance = new EventLogger(path);
            return Instance;
        }

        public void LogEvent(string message)
        {
            string timestampedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            RecentEvents.Insert(0, timestampedMessage);
            if (RecentEvents.Count > MaxEvents)
                RecentEvents.RemoveAt(RecentEvents.Count - 1);

            using var writer = new StreamWriter(_logFile, System.Text.Encoding.UTF8, 1024, leaveOpen: true);
            writer.WriteLine(timestampedMessage);
        }
    }
}
