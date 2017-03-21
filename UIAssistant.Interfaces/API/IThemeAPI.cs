using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Interfaces.API
{
    public interface IThemeAPI
    {
        IResourceItem CurrentTheme { get; }

        ISwitcher GetThemeSwitcher();
        void NextTheme();
        void SwitchTheme(string name);
    }
}