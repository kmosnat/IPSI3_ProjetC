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
        // Log d'un message de niveau INFO
        public static void LogInfo(this TextBox box, string message, LogSource source = LogSource.Client)
        {
            new Log(source, LogLevel.INFO, message)
                .AppendLog(
                    text => box.Invoke((MethodInvoker)(() => box.AppendText(text))),
                    source,
                    LogLevel.INFO,
                    message
                );
        }

        // Log d'un message de niveau WARNING
        public static void LogWarning(this TextBox box, string message, LogSource source = LogSource.Client)
        {
            new Log(source, LogLevel.WARNING, message)
                .AppendLog(
                    text => box.Invoke((MethodInvoker)(() => box.AppendText(text))),
                    source,
                    LogLevel.WARNING,
                    message
                );
        }

        // Log d'un message de niveau ERROR
        public static void LogError(this TextBox box, string message, LogSource source = LogSource.Client)
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
