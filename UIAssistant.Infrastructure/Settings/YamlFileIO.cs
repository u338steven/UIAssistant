using System;
using System.IO;
using YamlDotNet.Serialization;

using UIAssistant.Interfaces.Settings;

namespace UIAssistant.Infrastructure.Settings
{
    public class YamlFileIO : SettingsFileIOBase
    {
        Action<string, Exception> _exceptionHandler;

        public YamlFileIO(Action<string, Exception> handler = null)
        {
            _exceptionHandler = handler;
        }

        public override ISettings Read(Type type, params string[] pathParts)
        {
            var path = Combine(pathParts);
            try
            {
                if (!Exists(path))
                {
                    return Create(path, type);
                }
                using (var sr = new StreamReader(path))
                {
                    var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
                    return deserializer.Deserialize(sr, type) as ISettings;
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler?.Invoke(path, ex);
            }
            return null;
        }

        public override void Write(Type type, ISettings data, params string[] pathParts)
        {
            var path = Combine(pathParts);
            try
            {
                if (!Exists(path))
                {
                    CreateDirectory(path);
                }
                using (var sw = new StreamWriter(path))
                {
                    var serializer = new Serializer();
                    serializer.Serialize(sw, data, type);
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler?.Invoke(path, ex);
            }
        }
    }
}
