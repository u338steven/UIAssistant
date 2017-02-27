using System;
using System.IO;

namespace UIAssistant.Infrastructure.Logger
{
    public static class Log
    {
        public static void Error(Exception e, string message = null)
        {
            OutputStackTrace(e, "ERROR ", message);
        }

        public static void Fatal(Exception e, string message = null)
        {
            OutputStackTrace(e, "FATAL ", message);
        }

        private static void OutputStackTrace(Exception e, string header, string message)
        {
            if (!LogRotator.CanOutput)
            {
                return;
            }

            while (e != null)
            {
                Write(header + e.Message + "\n" + e.StackTrace + "\n");
                e = e.InnerException;
            }
        }

        public static void Warn(string message)
        {
            OutputMessage("WARN  ", message);
        }

        public static void Info(string message)
        {
            OutputMessage("INFO  ", message);
        }

        public static void Debug(string message)
        {
            OutputMessage("DEBUG ", message);
        }

        private static void OutputMessage(string header, string message)
        {
            if (!LogRotator.CanOutput)
            {
                return;
            }

            Write(header + message);
        }

        private static void Write(string message)
        {
            var header = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\t";
            try
            {
                using (var sw = new StreamWriter(LogRotator.FilePath, true) { NewLine = "\n" })
                {
                    sw.WriteLine(header + message.Replace("\r\n", "\n"));
                }
            }
            catch
            {
            }
        }
    }
}
