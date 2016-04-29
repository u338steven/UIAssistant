using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Reflection;
using YamlDotNet.Serialization;

using UIAssistant.UI.Controls;
using UIAssistant.Core.I18n;

namespace UIAssistant.Core.Settings
{
    public static class UIAssistantDirectory
    {
        public static string Executable => Directory.GetParent(Assembly.GetCallingAssembly().Location).ToString();
        public static string Plugins => Path.Combine(Executable, "Plugins");
        public static string Languages => Path.Combine(Executable, "Languages");
        public static string Themes => Path.Combine(Executable, "Themes");
        public static string Configurations => Path.Combine(Executable, "Configurations");
    }

    public interface ISettings
    {
        void Load();
        void Save();
    }

    public abstract class Settings<T> : ISettings where T : class, ISettings, new()
    {
        protected static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                    _instance.Load();
                }
                return _instance;
            }
        }

        protected string DirectoryPath => UIAssistantDirectory.Configurations;
        protected abstract string FileName { get; }
        protected string FilePath => Path.Combine(DirectoryPath, FileName);

        protected virtual T LoadDefault()
        {
            return new T();
        }

        protected abstract void LoadInternal();
        protected abstract void SaveInternal();

        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }
                File.Create(FilePath).Close();
                _instance = LoadDefault();
                _instance.Save();
                return;
            }

            try
            {
                LoadInternal();
            }
            catch(Exception ex)
            {
                _instance = LoadDefault();
                Notification.NotifyMessage("Load Settings Error", string.Format(TextID.SettingsLoadError.GetLocalizedText(), FileName), NotificationIcon.Warning);
                System.Diagnostics.Debug.Print(ex.Message);
#if DEBUG
                Logger.Log.Error(ex);
#endif
            }
        }

        public void Save()
        {
            SaveInternal();
        }
    }

    public abstract class YamlSettings<T> : Settings<T> where T : class, ISettings, new()
    {
        protected override void LoadInternal()
        {
            using (var sr = new StreamReader(FilePath))
            {
                var deserializer = new Deserializer(ignoreUnmatched: true);
                _instance = deserializer.Deserialize<T>(sr);
            }
        }

        protected override void SaveInternal()
        {
            using (var sw = new StreamWriter(FilePath))
            {
                var serializer = new Serializer();
                serializer.Serialize(sw, this);
            }
        }
    }
}
