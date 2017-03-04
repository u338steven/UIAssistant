using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;
using System.Windows;
using System.Globalization;

using UIAssistant.Infrastructure.Resource;
using UIAssistant.Infrastructure.Resource.Language;

namespace UIAssistant.Core.I18n
{
    public class Localizer : ILocalizer
    {
        public IList<Language> AvailableLanguages => _finder.Availables.Values.ToList();
        public Language CurrentLanguage => _state.Current;

        public string SuggestedCulture
        {
            get
            {
                string userCulture = CultureInfo.CurrentUICulture.Name;
                if (_finder.Find(userCulture) != null)
                {
                    return userCulture;
                }
                else
                {
                    return "en-US";
                }
            }
        }

        private ResourceFinder<Language> _finder;
        private ResourceState<Language> _state;

        public Localizer()
        {
            var reader = new ResourceReader<Language>();
            var dirPath = Path.Combine(Directory.GetParent(Assembly.GetCallingAssembly().Location).ToString(), "Languages");
            var resources = new ResourceDirectory<Language>(new LanguageKeyValueGenerator(), dirPath, "*.xaml");
            _finder = new ResourceFinder<Language>(resources);
            _state = new ResourceState<Language>(new ResourceUpdater<Language>(reader, Application.Current.Resources.MergedDictionaries));
        }

        public string GetLocalizedText(string key)
        {
            return _state.GetLocalizedText(key);
        }

        public void SwitchLanguage(Language language)
        {
            Application.Current.Dispatcher.Invoke(() => _state.Switch(_finder, language.Id));
        }

        public void SwitchNext()
        {
            Application.Current.Dispatcher.Invoke(() => _state.SwitchNext(_finder));
        }

        public Language FindLanguage(string culture)
        {
            return _finder.Find(culture);
        }
    }

    public static partial class ResourceExtensions
    {
        public static string GetLocalizedText(this ResourceState<Language> state, string key)
        {
            var translatedText = Application.Current.TryFindResource(key);
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
