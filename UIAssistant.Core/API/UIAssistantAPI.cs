using System;
using System.IO;
using System.Windows.Controls;

using UIAssistant.Core.Settings;
using UIAssistant.Infrastructure.Settings;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Settings;

namespace UIAssistant.Core.API
{
    public class UIAssistantAPI : IUIAssistantAPI
    {
        public static IUIAssistantAPI Instance { get; } = new UIAssistantAPI();
        public string ConfigurationDirectory { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configurations");
        public IFileIO DefaultSettingsFileIO { get; private set; }
        public IPluginManager PluginManager { get; private set; }
        public IScreen Screen { get { return new Utility.Screen(); } }
        public IUserSettings UIAssistantSettings { get; private set; }

        #region APIs
        public IAutomationAPI AutomationAPI { get; } = new AutomationAPI();
        public ICommandAPI CommandAPI { get; } = new CommandAPI();
        public IHitaHintAPI HitaHintAPI { get; } = new HitaHintAPI();
        public IKeyboardAPI KeyboardAPI { get; } = new KeyboardAPI();
        public ILocalizationAPI LocalizationAPI { get; } = new LocalizationAPI();
        public ILogAPI LogAPI { get; } = new LogAPI();
        public IMouseAPI MouseAPI { get; } = new MouseAPI();
        public INotificationAPI NotificationAPI { get; } = new NotificationAPI();
        public ISessionAPI SessionAPI { get; } = new SessionAPI();
        public IThemeAPI ThemeAPI { get; } = new ThemeAPI();
        public IViewAPI ViewAPI { get; private set; }
        public IWindowAPI WindowAPI { get; } = new WindowAPI();
        #endregion

        private UIAssistantAPI()
        {
            DefaultSettingsFileIO = new YamlFileIO((path, ex) => NotificationAPI.NotifyWarnMessage("Load Settings Error", string.Format(LocalizationAPI.Localize(TextID.SettingsLoadError), path)));
            UIAssistantSettings = DefaultSettingsFileIO.Read(typeof(UserSettings), UserSettings.FilePath) as UserSettings;
        }

        public void Initialize(Control defaultHUDPanel, Control defaultContextPanel)
        {
            ViewAPI = new ViewAPI(defaultHUDPanel, defaultContextPanel);
            PluginManager = Plugin.PluginManager.Instance;
        }
    }
}
