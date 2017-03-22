using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

using UIAssistant.Core.HitaHint;
using UIAssistant.Core.I18n;
using UIAssistant.Core.Settings;
using UIAssistant.Infrastructure.Events;
using UIAssistant.Infrastructure.Settings;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Events;
using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Resource;
using UIAssistant.Interfaces.Settings;

namespace UIAssistant.Core.API
{
    public class UIAssistantAPI : IUIAssistantAPI
    {
        public static IUIAssistantAPI Instance { get; } = new UIAssistantAPI();
        public string ConfigurationDirectory { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configurations");
        public IFileIO DefaultSettingsFileIO { get; private set; }
        public IPluginManager PluginManager { get; private set; }
        public IUserSettings UIAssistantSettings { get; private set; }

        private UIAssistantAPI()
        {
            DefaultSettingsFileIO = new YamlFileIO((path, ex) => NotificationAPI.NotifyWarnMessage("Load Settings Error", string.Format(Localize(TextID.SettingsLoadError), path)));
            UIAssistantSettings = DefaultSettingsFileIO.Read(typeof(UserSettings), UserSettings.FilePath) as UserSettings;
        }

        public void Initialize(Control defaultHUDPanel, Control defaultContextPanel)
        {
            ViewAPI = new ViewAPI(defaultHUDPanel, defaultContextPanel);
            PluginManager = Plugin.PluginManager.Instance;
        }

        public IEnumerable<string> GenerateHints(string hintKeys, int quantity)
        {
            if (hintKeys.Contains('|'))
            {
                return AlternateHintGenerator.Generate(hintKeys, quantity);
            }
            return HintGenerator.Generate(hintKeys, quantity);
        }

        public string Localize(string id)
        {
            return DefaultLocalizer.GetLocalizedText(id);
        }

       public IEventObserver GetObserver(ObserberKinds kind)
        {
            switch (kind)
            {
                case ObserberKinds.StructureChangedObserver:
                    return new StructureChangedObserver();
                case ObserberKinds.FocusObserver:
                    return new FocusObserver();
                case ObserberKinds.PopupObserver:
                    return new PopupObserver();
                default:
                    return null;
            }
        }

        public ILocalizer GetLocalizer()
        {
            return new Localizer(Directory.GetParent(System.Reflection.Assembly.GetCallingAssembly().Location).ToString());
        }

        public IResourceItem CurrentLanguage { get { return DefaultLocalizer.CurrentLanguage; } }
        public IScreen Screen { get { return new Utility.Screen(); } }

        public ICommandAPI CommandAPI { get; } = new CommandAPI();
        public IKeyboardAPI KeyboardAPI { get; } = new KeyboardAPI();
        public ILogAPI LogAPI { get; } = new LogAPI();
        public IMouseAPI MouseAPI { get; } = new MouseAPI();
        public INotificationAPI NotificationAPI { get; } = new NotificationAPI();
        public ISessionAPI SessionAPI { get; } = new SessionAPI();
        public IThemeAPI ThemeAPI { get; } = new ThemeAPI();
        public IViewAPI ViewAPI { get; private set; }
        public IWindowAPI WindowAPI { get; } = new WindowAPI();
    }
}
