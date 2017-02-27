using System.IO;
using YamlDotNet.Serialization;

namespace UIAssistant.Infrastructure.Settings
{
    public class YamlFile<T> : IFileIO<T> where T : ISettings
    {
        public string FilePath { get; }

        public YamlFile(string filePath)
        {
            FilePath = filePath;
        }

        public YamlFile(string directoryPath, string fileName)
        {
            FilePath = Path.Combine(directoryPath, fileName);
        }

        public T Read()
        {
            using (var sr = new StreamReader(FilePath))
            {
                var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
                return deserializer.Deserialize<T>(sr);
            }
        }

        public void Write(T data)
        {
            using (var sw = new StreamWriter(FilePath))
            {
                var serializer = new Serializer();
                serializer.Serialize(sw, data);
            }
        }
    }
}
