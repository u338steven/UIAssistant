using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using UIAssistant.Interfaces.Settings;

namespace SettingsTest
{
    public static class TestPath
    {
        public static string DirName { get; } = @"UIAssistantTest";
        public static string FileName { get; } = @"foo.yml";
        public static string FilePath { get; } = Path.Combine(Path.GetTempPath(), DirName, FileName);
    }

    public static class FoobarPath
    {
        public static string DirName { get; } = @"UIAssistantTest";
        public static string SubDirName { get; } = @"Foobar";
        public static string DirPath { get; } = Path.Combine(Path.GetTempPath(), DirName, SubDirName);
        public static string FileName { get; } = @"foobar.yml";
        public static string FilePath { get; } = Path.Combine(DirPath, FileName);
    }

    public class TestStorage : ISettings
    {
        public bool UseMigemo { get; set; }
        public string Theme { get; set; }
        public int ItemsCountPerPage { get; set; }

        public void SetValuesDefault()
        {
            UseMigemo = true;
            Theme = "General";
            ItemsCountPerPage = 15;
        }
    }

    public class ThemeRemoved : ISettings
    {
        public bool UseMigemo { get; set; }
        public int ItemsCountPerPage { get; set; }

        public void SetValuesDefault()
        {
            UseMigemo = true;
            ItemsCountPerPage = 15;
        }
    }

    public class ExceptionRaised : ISettings
    {
        // should be bool, but int => raise exception
        public int UseMigemo { get; set; }
        public int ItemsCountPerPage { get; set; }

        public void SetValuesDefault()
        {
            UseMigemo = 33;
            ItemsCountPerPage = 15;
        }
    }

    public class FooSettings : ISettings
    {
        public int Foo { get; set; }
        public int Bar { get; set; }
        private int FooBar { get; } = 100;

        public void SetValuesDefault()
        {
            Foo = 100;
            Bar = 200;
        }
    }

    public class SettingsLoader<T> where T : class, ISettings
    {
        public T Settings { get; private set; }
        private IFileIO _fileIO;
        private string _filePath;

        public SettingsLoader(IFileIO fileIO, string filePath)
        {
            _fileIO = fileIO;
            _filePath = filePath;
            Load();
        }

        public void Save()
        {
            _fileIO.Write(typeof(T), Settings, _filePath);
        }

        public void Load()
        {
            Settings = _fileIO.Read(typeof(T), _filePath) as T;
        }
    }

    public class SettingsLoader
    {
        public FooSettings Settings { get; private set; }
        private IFileIO _fileIO;
        private string _filePath = FoobarPath.FilePath;

        public SettingsLoader(IFileIO fileIO)
        {
            _fileIO = fileIO;
            Load();
        }

        public void Save()
        {
            _fileIO.Write(typeof(FooSettings), Settings, _filePath);
        }

        public void Load()
        {
            Settings = _fileIO.Read(typeof(FooSettings), _filePath) as FooSettings;
        }
    }
}
