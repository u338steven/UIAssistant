using System;
using System.Linq;
using System.IO;

namespace UIAssistant.Core.Logger
{
    public static class Log
    {
        private static string _logFile;
        private static string _logDirectory;
        private static bool _canOutputLog = true;

        const int YEAR = 0;
        const int MONTH = 1;
        const int DAY = 2;

        static Log()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

            if (!Directory.Exists(_logDirectory))
            {
                try
                {
                    Directory.CreateDirectory(_logDirectory);
                }
                catch
                {
                    _canOutputLog = false;
                    return;
                }
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

        private static bool CanOutput()
        {
            return _canOutputLog;
        }

        public static void Error(Exception e, string message = null)
        {
            if (!CanOutput())
            {
                return;
            }

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
            if (!CanOutput())
            {
                return;
            }

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
            if (!CanOutput())
            {
                return;
            }

            Initialize();
            Write("WARN " + message);
        }

        public static void Info(string message)
        {
            if (!CanOutput())
            {
                return;
            }

            Initialize();
            Write("INFO " + message);
        }

        public static void Debug(string message)
        {
            if (!CanOutput())
            {
                return;
            }

            Initialize();
            Write("DEBUG " + message);
        }
    }
}
