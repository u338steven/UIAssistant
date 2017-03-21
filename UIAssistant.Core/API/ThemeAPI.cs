using System.IO;
using UIAssistant.Core.Themes;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Core.API
{
    class ThemeAPI : IThemeAPI
    {
        public void SwitchTheme(string name)
        {
            UIAssistantAPI.Instance.UIDispatcher.Invoke(() =>
            {
                ThemeDefaultSwitcher.Switch(name);
            });
        }

        public void NextTheme()
        {
            UIAssistantAPI.Instance.UIDispatcher.Invoke(() =>
            {
                ThemeDefaultSwitcher.Next();
            });
        }

        public IResourceItem CurrentTheme
        {
            get
            {
                return ThemeDefaultSwitcher.CurrentTheme;
            }
        }

        public ISwitcher GetThemeSwitcher()
        {
            return new ThemeSwitcher(Directory.GetParent(System.Reflection.Assembly.GetCallingAssembly().Location).ToString());
        }
    }
}
