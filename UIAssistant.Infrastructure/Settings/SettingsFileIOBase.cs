using System;
using System.IO;

using UIAssistant.Interfaces.Settings;

namespace UIAssistant.Infrastructure.Settings
{
    public abstract class SettingsFileIOBase : IFileIO
    {
        public abstract ISettings Read(Type type, params string[] pathParts);
        public abstract void Write(Type type, ISettings data, params string[] pathParts);

        protected ISettings Create(string path, Type type)
        {
            CreateDirectory(path);
            var instance = type.InvokeMember(null, System.Reflection.BindingFlags.CreateInstance, null, null, null) as ISettings;
            instance.SetValuesDefault();
            Write(type, instance, path);
            return instance;
        }

        protected void CreateDirectory(string path)
        {
            var directoryPath = Directory.GetParent(path).FullName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        protected string Combine(params string[] pathParts)
        {
            try
            {
                return Path.Combine(pathParts);
            }
            catch
            {
                return "";
            }
        }
    }
}
