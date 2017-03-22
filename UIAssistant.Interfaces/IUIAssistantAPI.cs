using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using KeybindHelper.LowLevel;
using UIAssistant.Interfaces.Events;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Resource;
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
        string Localize(string id);
        IPluginManager PluginManager { get; }

        IEventObserver GetObserver(ObserberKinds kind);
        ILocalizer GetLocalizer();
        IResourceItem CurrentLanguage { get; }

        IScreen Screen { get; }

        ICommandAPI CommandAPI { get; }
        IKeyboardAPI KeyboardAPI { get; }
        ILogAPI LogAPI { get; }
        IMouseAPI MouseAPI { get; }
        INotificationAPI NotificationAPI { get; }
        ISessionAPI SessionAPI { get; }
        IThemeAPI ThemeAPI { get; }
        IViewAPI ViewAPI { get; }
        IWindowAPI WindowAPI { get; }
    }
}
