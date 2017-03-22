using System.Collections.Generic;
using System.Windows.Controls;
using UIAssistant.Interfaces.Events;
using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Settings;

namespace UIAssistant.Interfaces.API
{
    public interface IUIAssistantAPI
    {
        string ConfigurationDirectory { get; }
        IFileIO DefaultSettingsFileIO { get; }
        IUserSettings UIAssistantSettings { get; }

        IEnumerable<string> GenerateHints(string hintKeys, int quantity);
        void Initialize(Control defaultHUDPanel, Control defaultContextPanel);
        IPluginManager PluginManager { get; }

        IEventObserver GetObserver(ObserberKinds kind);
        IScreen Screen { get; }
        ICommandAPI CommandAPI { get; }
        IKeyboardAPI KeyboardAPI { get; }
        ILocalizationAPI LocalizationAPI { get; }
        ILogAPI LogAPI { get; }
        IMouseAPI MouseAPI { get; }
        INotificationAPI NotificationAPI { get; }
        ISessionAPI SessionAPI { get; }
        IThemeAPI ThemeAPI { get; }
        IViewAPI ViewAPI { get; }
        IWindowAPI WindowAPI { get; }
    }
}
