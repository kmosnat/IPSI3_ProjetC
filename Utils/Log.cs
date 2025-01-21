using System;
using System.Windows;

namespace Utils
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

        public void AppendLog(Action<string> appendAction, LogSource source, LogLevel level, string message)
        {
            var logEntry = new Log(source, level, message);
            string content = logEntry.ToString().Replace("\n", Environment.NewLine);
            string finalMessage = "--------------------------" + Environment.NewLine
                                  + content + Environment.NewLine;

            appendAction(finalMessage);
        }
    }

}
