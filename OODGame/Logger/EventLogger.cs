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
        public string FilePath => _path;
        private readonly FileStream _logFile;
        public static EventLogger? Instance { get; private set; }

        private EventLogger(string path)
        {
            RecentEvents = new List<string>();
            _path = path;


            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            _logFile = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _logFile.Seek(0, SeekOrigin.End);
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
            RecentEvents.Insert(0, message);
            if (RecentEvents.Count > MaxEvents)
                RecentEvents.RemoveAt(RecentEvents.Count - 1);

            Draw.DrawRecentLogs();
            _logFile.Seek(0, SeekOrigin.End);
            using var writer = new StreamWriter(_logFile, System.Text.Encoding.UTF8, 1024, leaveOpen: true);
            writer.WriteLine(timestampedMessage);
        }
        public void ViewAllLogs()
        {
            List<string> lines = new List<string>();

            using var writer = new StreamWriter(_logFile, System.Text.Encoding.UTF8, 1024, leaveOpen: true);
            writer.Flush();
            long previousPosition = _logFile.Position;
            _logFile.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(_logFile, System.Text.Encoding.UTF8, leaveOpen: true))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                    lines.Add(line);
            }
            _logFile.Seek(previousPosition, SeekOrigin.Begin);

            lines.Reverse();

            if (lines.Count == 0)
                lines.Add("(log file not found)");

            int visibleRows = Console.WindowHeight - 3;
            int scroll = 0;

            Draw.DrawLogViewer(lines, scroll, visibleRows);

            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                    break;
                if (key == ConsoleKey.UpArrow && scroll > 0)
                { scroll--; Draw.DrawLogViewer(lines, scroll, visibleRows); }
                else if (key == ConsoleKey.DownArrow && scroll < lines.Count - visibleRows)
                { scroll++; Draw.DrawLogViewer(lines, scroll, visibleRows); }
            }
        }
    }
}
