using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Interfaces.API
{
    public interface ILocalizationAPI
    {
        IResourceItem CurrentLanguage { get; }

        ILocalizer GetLocalizer();
        string Localize(string id);
    }
}