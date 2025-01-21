using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Serveur
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }

    public static class TextBoxLogExtensions
    {
        /// <summary>
        /// Log d'un message de niveau INFO
        /// </summary>
        public static void LogInfo(this TextBox box, string message, LogSource source = LogSource.Serveur)
        {
            new Log(source, LogLevel.INFO, message)
                .AppendLog(
                    text => box.Invoke((MethodInvoker)(() => box.AppendText(text))),
                    source,
                    LogLevel.INFO,
                    message
                );
        }

        /// <summary>
        /// Log d'un message de niveau WARNING
        /// </summary>
        public static void LogWarning(this TextBox box, string message, LogSource source = LogSource.Serveur)
        {
            new Log(source, LogLevel.WARNING, message)
                .AppendLog(
                    text => box.Invoke((MethodInvoker)(() => box.AppendText(text))),
                    source,
                    LogLevel.WARNING,
                    message
                );
        }

        /// <summary>
        /// Log d'un message de niveau ERROR
        /// </summary>
        public static void LogError(this TextBox box, string message, LogSource source = LogSource.Serveur)
        {
            new Log(source, LogLevel.ERROR, message)
                .AppendLog(
                    text => box.Invoke((MethodInvoker)(() => box.AppendText(text))),
                    source,
                    LogLevel.ERROR,
                    message
                );
        }
    }
}
