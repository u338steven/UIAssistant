using System.IO;

using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Infrastructure.Resource.Theme
{
    public class ThemeKeyValueGenerator : IResourceKeyValueGenerator<Theme>
    {
        public string GenerateKey(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public Theme GenerateValue(string filePath)
        {
            return new Theme(GenerateKey(filePath), filePath);
        }
    }
}
