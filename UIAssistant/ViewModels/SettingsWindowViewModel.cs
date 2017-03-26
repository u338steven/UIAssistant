using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;

using UIAssistant.Models;
using UIAssistant.Core.API;
using UIAssistant.Core.Plugin;
using UIAssistant.Infrastructure.Commands;
using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Settings;

namespace UIAssistant.ViewModels
{
    public class SettingsWindowViewModel : ViewModel
    {
        #region Settings変更通知プロパティ
        private IUserSettings _Settings;

        public IUserSettings Settings
        {
            get
            { return _Settings; }
            set
            {
                if (_Settings == value)
                    return;
                _Settings = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public string Version { get; private set; }

        public SettingsWindowViewModel()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            Version = $"UIAssistant {version}";
        }

        public void Initialize()
        {
            Settings = UIAssistantAPI.Instance.UIAssistantSettings;
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var plugin in PluginManager.Instance.Plugins)
            {
                (plugin.Value as IConfigurablePlugin)?.Save();
            }
            Settings.Save();

            CommandManager.Clear();
            PluginManager.Instance.ResetAllPlugins();
            Hotkey.RegisterHotkeys();

            base.Dispose(disposing);
        }
    }
}
