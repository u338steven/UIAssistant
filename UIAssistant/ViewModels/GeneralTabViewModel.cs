using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;

using UIAssistant.Models;
using UIAssistant.Core.API;
using UIAssistant.Core.I18n;
using UIAssistant.Core.Tools;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Settings;
using UIAssistant.Infrastructure.Resource.Language;
using UIAssistant.Utility;

namespace UIAssistant.ViewModels
{
    public class GeneralTabViewModel : ViewModel
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

        #region Language変更通知プロパティ
        private Language _Language;

        public Language Language
        {
            get
            { return _Language; }
            set
            {
                if (_Language == value)
                    return;
                _Language = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region RunAtLogin変更通知プロパティ
        private bool _RunAtLogin;

        public bool RunAtLogin
        {
            get
            { return _RunAtLogin; }
            set
            {
                if (_RunAtLogin == value)
                    return;
                _RunAtLogin = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region UseMigemo変更通知プロパティ
        private bool _UseMigemo;

        public bool UseMigemo
        {
            get
            { return _UseMigemo; }
            set
            { 
                if (_UseMigemo == value)
                    return;
                _UseMigemo = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public IList<Language> Languages { get { return DefaultLocalizer.AvailableLanguages; } }

        public void Initialize()
        {
            Settings = UIAssistantAPI.Instance.UIAssistantSettings;
            Language = DefaultLocalizer.FindLanguage(Settings.Culture);
            RunAtLogin = Settings.RunAtLogin;
            UseMigemo = Settings.UseMigemo;
            LocalizeKeybindsText();
        }

        private void LocalizeKeybindsText()
        {
            Settings.Quit.Text = TextID.KeybindsQuit.GetLocalizedText();
            Settings.Back.Text = TextID.KeybindsBack.GetLocalizedText();
            Settings.Clear.Text = TextID.KeybindsClear.GetLocalizedText();
            Settings.Delete.Text = TextID.KeybindsDelete.GetLocalizedText();
            Settings.Execute.Text = TextID.KeybindsExecute.GetLocalizedText();
            Settings.ShowExtraActions.Text = TextID.KeybindsShowExtraActions.GetLocalizedText();
            Settings.Left.Text = TextID.KeybindsLeft.GetLocalizedText();
            Settings.Right.Text = TextID.KeybindsRight.GetLocalizedText();
            Settings.Up.Text = TextID.KeybindsUp.GetLocalizedText();
            Settings.Down.Text = TextID.KeybindsDown.GetLocalizedText();
            Settings.PageUp.Text = TextID.KeybindsPageUp.GetLocalizedText();
            Settings.PageDown.Text = TextID.KeybindsPageDown.GetLocalizedText();
            Settings.Home.Text = TextID.KeybindsHome.GetLocalizedText();
            Settings.End.Text = TextID.KeybindsEnd.GetLocalizedText();
            Settings.TemporarilyHide.Text = TextID.KeybindsTemporarilyHide.GetLocalizedText();
            Settings.SwitchKeyboardLayout.Text = TextID.KeybindsSwitchKeyboardLayout.GetLocalizedText();
            Settings.SwitchTheme.Text = TextID.KeybindsSwitchTheme.GetLocalizedText();
            Settings.Usage.Text = TextID.KeybindsUsage.GetLocalizedText();
            Settings.EmergencySwitch.Text = TextID.KeybindsEmergencySwitch.GetLocalizedText();
        }

        public void OnLanguageChanged()
        {
            Settings.Culture = Language.Id;
            DefaultLocalizer.SwitchLanguage(Language);
            LocalizeKeybindsText();
            UIAssistantAPI.Instance.PluginManager.Localize();
            TasktrayIcon.HideNotifyIcon();
            TasktrayIcon.ShowNotifyIcon();
        }

        public void OnRunAtLoginChecked()
        {
            Settings.RunAtLogin = true;
            AutoRunAtLoginScheduler.Register();
        }

        public void OnRunAtLoginUnchecked()
        {
            Settings.RunAtLogin = false;
            AutoRunAtLoginScheduler.Unregister();
        }

        public void OnUseMigemoChecked()
        {
            Settings.UseMigemo = true;
            try
            {
                Migemo.Initialize(Settings.MigemoDllPath, Settings.MigemoDictionaryPath);
            }
            catch (Exception ex)
            {
                UIAssistantAPI.Instance.NotificationAPI.NotifyWarnMessage("Load Migemo Error", $"{ex.Message}");
            }
        }

        public void OnUseMigemoUnchecked()
        {
            Settings.UseMigemo = false;
            Migemo.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
