using System;
using System.Windows;

namespace Utils
{
    // Enumération pour les sources de log
    public enum LogSource
    {
        Arduino,
        Client,
        Serveur,
        Camera
    }

    // Enumération pour les niveaux de log
    public enum LogLevel
    {
        INFO,
        WARNING,
        ERROR
    }

    // Classe pour représenter un log
    public class Log
    {
        public LogSource Source { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }

        // Constructeur
        public Log(LogSource source, LogLevel level, string message)
        {
            Source = source;
            Level = level;
            Message = message;
            Time = DateTime.Now;
        }

        // Méthode retournant une représentation textuelle du log
        public override string ToString()
        {
            return $"{Time}:\n[{Source}][{Level}] - {Message}";
        }

        // Méthode pour afficher le log dans une MessageBox
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
