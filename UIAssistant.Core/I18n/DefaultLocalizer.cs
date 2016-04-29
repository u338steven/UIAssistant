using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIAssistant.Core.I18n
{
    public static class DefaultLocalizer
    {
        static Localizer _localizer = new Localizer();

        public static string GetLocalizedText(this string id)
        {
            return _localizer.GetLocalizedText(id);
        }

        public static void SwitchLanguage(Language language)
        {
            _localizer.SwitchLanguage(language);
        }

        public static Language CurrentLanguage => _localizer.CurrentLanguage;

        public static Language FindLanguage(string culture)
        {
            return _localizer.FindLanguage(culture);
        }

        public static string SuggestedCulture
        {
            get
            {
                return _localizer.SuggestedCulture;
            }
        }

        public static IList<Language> AvailableLanguages
        {
            get
            {
                return _localizer.AvailableLanguages;
            }
        }
    }
}
