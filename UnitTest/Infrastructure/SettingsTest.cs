using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using UIAssistant.Infrastructure.Settings;
using UIAssistant.Interfaces.Settings;

namespace SettingsTest
{
    public static class TestPath
    {
        public static string DirName { get; } = @"UIAssistantTest";
        public static string FileName { get; } = @"foo.yml";
        public static string FilePath { get; } = Path.Combine(Path.GetTempPath(), DirName, FileName);
    }

    public class TestStorage : Settings<TestStorage>
    {
        public bool UseMigemo { get; set; }
        public string Theme { get; set; }
        public int ItemsCountPerPage { get; set; }

        private IFileIO<TestStorage> _fileIO = new YamlFile<TestStorage>(TestPath.FilePath);
        protected override IFileIO<TestStorage> FileIO { get { return _fileIO; }}

        protected override void LoadDefault()
        {
            UseMigemo = true;
            Theme = "General";
            ItemsCountPerPage = 15;
        }
    }

    public class ThemeRemoved : Settings<ThemeRemoved>
    {
        public bool UseMigemo { get; set; }
        public int ItemsCountPerPage { get; set; }

        private IFileIO<ThemeRemoved> _fileIO = new YamlFile<ThemeRemoved>(TestPath.FilePath);
        protected override IFileIO<ThemeRemoved> FileIO { get { return _fileIO; } }

        protected override void LoadDefault()
        {
            UseMigemo = true;
            ItemsCountPerPage = 15;
        }
    }

    public class ExceptionRaised : Settings<ExceptionRaised>
    {
        // should be bool, but int => raise exception
        public int UseMigemo { get; set; }
        public int ItemsCountPerPage { get; set; }

        private IFileIO<ExceptionRaised> _fileIO = new YamlFile<ExceptionRaised>(TestPath.FilePath);
        protected override IFileIO<ExceptionRaised> FileIO { get { return _fileIO; } }

        public ExceptionRaised()
        {
            OnError = ex =>
            {
                throw ex;
            };
        }

        protected override void LoadDefault()
        {
            UseMigemo = 33;
            ItemsCountPerPage = 15;
        }
    }
}
