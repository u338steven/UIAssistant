using System;
using System.IO;

using UIAssistant.Interfaces.Settings;

namespace UIAssistant.Infrastructure.Settings
{
    public abstract class Settings<T> : ISettings where T : class, ISettings, new()
    {
        private static T _instance;
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

        protected Action<Exception> OnError { get; set; }
        protected abstract IFileIO<T> FileIO { get; }

        protected abstract void LoadDefault();

        public void Load()
        {
            if (!File.Exists(FileIO.FilePath))
            {
                var directoryPath = Directory.GetParent(FileIO.FilePath).FullName;
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                LoadDefault();
                Save();
                return;
            }

            try
            {
                _instance = FileIO.Read();
            }
            catch (Exception ex)
            {
                LoadDefault();
                OnError?.Invoke(ex);
            }
        }

        public void Save()
        {
            FileIO.Write(_instance);
        }
    }
}
