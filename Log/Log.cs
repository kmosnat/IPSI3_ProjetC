using System;

namespace Log
{
    public enum LogSource
    {
        Arduino,
        Client,
        Serveur,
        Camera
    }

    public enum LogLevel
    {
        INFO,
        WARNING,
        ERROR
    }

    public class Log
    {
        public LogSource Source { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }

        public Log(LogSource source, LogLevel level, string message)
        {
            Source = source;
            Level = level;
            Message = message;
            Time = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Time}:\n[{Source}][{Level}] - {Message}";
        }
    }
}
