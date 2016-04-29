using System;
using System.Linq;
using System.IO;

namespace UIAssistant.Core.Logger
{
    public static class Log
    {
        private static string _logFile;
        private static string _logDirectory;

        const int YEAR = 0;
        const int MONTH = 1;
        const int DAY = 2;

        static Log()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            RemoveOldLog();
        }

        private static void RemoveOldLog()
        {
            var now = DateTime.Now;
            var dir = new DirectoryInfo(_logDirectory);
            var files = dir.EnumerateFiles("*.log", SearchOption.TopDirectoryOnly);
            foreach (var f in files)
            {
                var dateString = Path.GetFileNameWithoutExtension(f.Name).Split('-');
                if (dateString.Count() != 3)
                {
                    continue;
                }
                int[] date;
                try
                {
                    date = dateString.Select(x => Convert.ToInt32(x)).ToArray();
                }
                catch
                {
                    continue;
                }
                var fileDate = new DateTime(date[YEAR], date[MONTH], date[DAY]);
                if (now.Subtract(fileDate) > TimeSpan.FromDays(30))
                {
                    f.Delete();
                }
            }
        }

        private static void Initialize()
        {
            var now = DateTime.Now;
            _logFile = Path.Combine(_logDirectory, now.ToString("yyyy-MM-dd") + ".log");
        }

        private static void Write(string message)
        {
            var now = DateTime.Now;
            var header = now.ToString("yyyy/MM/dd HH:mm:ss") + "\t";
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(_logFile, true);
                sw.WriteLine(header + message);
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }

        public static void Error(Exception e, string message = null)
        {
            Initialize();
            while (e != null)
            {
                Write("ERROR " + e.Message);
                Write("ERROR " + e.StackTrace);
                e = e.InnerException;
            }
        }

        public static void Fatal(Exception e, string message = null)
        {
            Initialize();
            while (e != null)
            {
                Write("FATAL " + e.Message);
                Write("FATAL " + e.StackTrace);
                e = e.InnerException;
            }
        }

        public static void Warn(string message)
        {
            Initialize();
            Write("WARN " + message);
        }

        public static void Info(string message)
        {
            Initialize();
            Write("INFO " + message);
        }

        public static void Debug(string message)
        {
            Initialize();
            Write("DEBUG " + message);
        }
    }
}
