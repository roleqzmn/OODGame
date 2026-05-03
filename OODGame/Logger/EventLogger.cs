using OODGame.Items;
using OODGame.Map;
using OODGame.Entities;
using System;
using System.Collections.Generic;
using OODGame.Players;
using OODGame.Actions;
using OODGame;
namespace OODGame.Logger
{
    public sealed class EventLogger : IEventLogger
    {
        public List<string> RecentEvents { get; private set; }
        public int MaxEvents { get; private set; } = 8;
        public string Path { private get; set; }
        public FileStream LogFile { get; private set; }
        public static EventLogger? Instance { get; private set; }
        private EventLogger(string path)
        {
            RecentEvents = new List<string>();
            Path = path;
            LogFile = new FileStream(Path, FileMode.Append, FileAccess.Write);
        }

        public static EventLogger GetInstance(string path)
        {
            if (Instance == null)
            {
                Instance = new EventLogger(path);
            }
            return Instance;
        }
        public void LogEvent(string message)
        {
            string timestampedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            RecentEvents.Insert(0, timestampedMessage);
            if (RecentEvents.Count > MaxEvents)
            {
                RecentEvents.RemoveAt(RecentEvents.Count - 1);
            }
            using (StreamWriter writer = new StreamWriter(LogFile, System.Text.Encoding.UTF8, 1024, true))
            {
                writer.WriteLine(timestampedMessage);
            }
        }
    }
}