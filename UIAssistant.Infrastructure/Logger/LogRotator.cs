using System;
using System.Linq;
using System.IO;

namespace UIAssistant.Infrastructure.Logger
{
    public static class LogRotator
    {
        public static string DirectoryPath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        public static string FilePath { get; } = Path.Combine(DirectoryPath, DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        public static bool CanOutput { get { return Directory.Exists(DirectoryPath); } }

        static LogRotator()
        {
            if (!Directory.Exists(DirectoryPath))
            {
                try
                {
                    Directory.CreateDirectory(DirectoryPath);
                }
                catch
                {
                    return;
                }
            }
            RemoveOldLog();
        }

        const int YEAR = 0;
        const int MONTH = 1;
        const int DAY = 2;

        // delete logs older than 30 days
        private static void RemoveOldLog()
        {
            var now = DateTime.Now;
            var dir = new DirectoryInfo(DirectoryPath);
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
    }
}
