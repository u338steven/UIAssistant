using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;
using System.IO;
using System.Reflection;

namespace UIAssistant.Core.I18n
{
    public class Localizer
    {
        private Translator _translator = new Translator(Directory.GetParent(Assembly.GetCallingAssembly().Location).ToString());

        public string GetLocalizedText(string id)
        {
            return _translator.Translate(id);
        }

        public void SwitchLanguage(Language language)
        {
            _translator.Switch(language);
        }

        public Language CurrentLanguage => _translator.Current;

        public Language FindLanguage(string culture)
        {
            return _translator.Find(culture);
        }

        public string SuggestedCulture
        {
            get
            {
                string userCulture = CultureInfo.CurrentUICulture.Name;
                if (_translator.Find(userCulture) != null)
                {
                    return userCulture;
                }
                else
                {
                    return "en-US";
                }
            }
        }

        public IList<Language> AvailableLanguages
        {
            get
            {
                return _translator.GetAvailables();
            }
        }
    }
}
