using System.IO;
using UIAssistant.Core.I18n;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Core.API
{
    class LocalizationAPI : ILocalizationAPI
    {
        public IResourceItem CurrentLanguage { get { return DefaultLocalizer.CurrentLanguage; } }

        public string Localize(string id)
        {
            return DefaultLocalizer.GetLocalizedText(id);
        }

        public ILocalizer GetLocalizer()
        {
            return new Localizer(Directory.GetParent(System.Reflection.Assembly.GetCallingAssembly().Location).ToString());
        }
    }
}
