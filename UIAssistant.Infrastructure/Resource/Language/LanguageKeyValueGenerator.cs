using System.IO;
using System.Globalization;

using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Infrastructure.Resource.Language
{
    public class LanguageKeyValueGenerator : IResourceKeyValueGenerator<Language>
    {
        public string GenerateKey(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public Language GenerateValue(string filePath)
        {
            var name = Path.GetFileNameWithoutExtension(filePath);
            string displayName;
            try
            {
                displayName = CultureInfo.GetCultureInfo(name).DisplayName;
            }
            catch
            {
                displayName = name;
            }
            return new Language(name, filePath, displayName);
        }
    }
}
