using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.IO;
using System.Globalization;
using UIAssistant.Core.Resource;

namespace UIAssistant.Core.I18n
{
    public class Translator : Resource<Language>
    {
        public override string Default => "en-US";
        protected override string DirectoryPath => _directoryPath;
        private string _directoryPath;

        public Translator(string rootDirectory)
        {
            _directoryPath = Path.Combine(rootDirectory, "Languages");
            Switch(Find(CultureInfo.CurrentUICulture.Name));
        }

        protected override void CreateAvailableDictionaries(string[] files)
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            var candidates = files.Where(culture => cultures.Any(c => c.Name == culture)).ToList();

            AvailableDictionaries = candidates.Select(c => new Language(c, CultureInfo.GetCultureInfo(c).DisplayName)).ToList();
        }

        public string Translate(string id)
        {
            var translatedText = Application.Current.TryFindResource(id);
            if (translatedText is string)
            {
                return translatedText as string;
            }
            else
            {
                return "Laguage files may be broken. Please reinstall.";
            }
        }
    }
}
