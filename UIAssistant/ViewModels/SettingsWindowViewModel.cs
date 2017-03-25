﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using System.Windows;
using System.Windows.Controls;

using UIAssistant.Models;
using UIAssistant.Core.API;
using UIAssistant.Core.I18n;
using UIAssistant.Core.Plugin;
using UIAssistant.Core.Tools;
using UIAssistant.Infrastructure.Commands;
using UIAssistant.Infrastructure.Resource.Language;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Commands;
using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Settings;
using UIAssistant.Utility;

using UIAssistant.UI.Controls;

using KeybindHelper;

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
                OnLanguageChanged();
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
                OnRunAtLoginChanged();
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
                OnUseMigemo();
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Hotkeys変更通知プロパティ
        private ObservableSynchronizedCollection<Keybind> _Hotkeys = new ObservableSynchronizedCollection<Keybind>();

        public ObservableSynchronizedCollection<Keybind> Hotkeys
        {
            get
            { return _Hotkeys; }
            set
            {
                if (_Hotkeys == value)
                    return;
                _Hotkeys = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Generator変更通知プロパティ
        private ICandidatesGenerator _Generator;

        public ICandidatesGenerator Generator
        {
            get
            { return _Generator; }
            set
            {
                if (_Generator == value)
                    return;
                _Generator = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Validator変更通知プロパティ
        private IValidatable<string> _Validator;

        public IValidatable<string> Validator
        {
            get
            { return _Validator; }
            set
            { 
                if (_Validator == value)
                    return;
                _Validator = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Version変更通知プロパティ
        private string _Version;

        public string Version
        {
            get
            { return _Version; }
            set
            {
                if (_Version == value)
                    return;
                _Version = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CanClose変更通知プロパティ
        private bool _CanClose;
        public bool CanClose
        {
            get { return _CanClose; }
            set
            {
                if (_CanClose == value) { return; }

                _CanClose = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public IList<Language> Languages { get { return DefaultLocalizer.AvailableLanguages; } }

        private bool _isInitialized;

        public void Initialize()
        {
            Settings = UIAssistantAPI.Instance.UIAssistantSettings;
            Language = DefaultLocalizer.FindLanguage(Settings.Culture);
            RunAtLogin = Settings.RunAtLogin;
            UseMigemo = Settings.UseMigemo;
            Hotkeys = new ObservableSynchronizedCollection<Keybind>(Settings.Commands);

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            Version = $"UIAssistant {version}";

            Generator = CommandManager.GetGenerator();
            Validator = CommandManager.GetValidator(DefaultLocalizer.Instance);
            LocalizeKeybindsText();
            _isInitialized = true;
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

        public void AddHotkey(HotkeyWithCommandListBox hotkeys)
        {
            Hotkeys.Add(new Keybind());
            hotkeys.SelectedIndex = Hotkeys.Count - 1;
            hotkeys.ScrollIntoView(hotkeys.SelectedItem);
            hotkeys.UpdateLayout();
            hotkeys.FocusOnKeybindBox(hotkeys.SelectedIndex);
            Settings.Commands = Hotkeys.ToList();
        }

        public void RemoveHotkey(HotkeyWithCommandListBox hotkeys)
        {
            var selectedIndex = hotkeys.SelectedIndex;
            if (selectedIndex < 0)
            {
                return;
            }
            Hotkeys.RemoveAt(selectedIndex);
            hotkeys.UpdateLayout();
            hotkeys.SelectedIndex = Hotkeys.Count - 1;
            Settings.Commands = Hotkeys.ToList();
        }

        private void OnLanguageChanged()
        {
            if (!_isInitialized)
            {
                return;
            }
            Settings.Culture = Language.Id;
            DefaultLocalizer.SwitchLanguage(Language);
            LocalizeKeybindsText();
            PluginManager.Instance.Localize();
            TasktrayIcon.HideNotifyIcon();
            TasktrayIcon.ShowNotifyIcon();
        }

        private void OnRunAtLoginChanged()
        {
            if (!_isInitialized)
            {
                return;
            }
            Settings.RunAtLogin = RunAtLogin;
            if (RunAtLogin)
            {
                AutoRunAtLoginScheduler.Register();
            }
            else
            {
                AutoRunAtLoginScheduler.Unregister();
            }
        }

        private void OnUseMigemo()
        {
            if (!_isInitialized)
            {
                return;
            }
            Settings.UseMigemo = UseMigemo;
            if (UseMigemo)
            {
                try
                {
                    Migemo.Initialize(Settings.MigemoDllPath, Settings.MigemoDictionaryPath);
                }
                catch (Exception ex)
                {
                    UIAssistantAPI.Instance.NotificationAPI.NotifyWarnMessage("Load Migemo Error", $"{ex.Message}");
                }
            }
            else
            {
                Migemo.Dispose();
            }
        }

        public void CloseCanceledCallback()
        {
            if (!Settings.Commands.Any(x => string.IsNullOrEmpty(x.Text))
                && !Settings.Commands.Where(x => !string.IsNullOrEmpty(x.Text)).Any(x => Validator.Validate(x.Text) != null))
            {
                CanClose = true;
                DispatcherHelper.UIDispatcher.BeginInvoke((Action)(() =>
                {
                    Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction"));
                }));
                return;
            }

            var message = new ConfirmationMessage(
                $"{TextID.HotkeyHasError.GetLocalizedText()} \n{TextID.CloseConfirmation.GetLocalizedText()}",
                "Confirmation",
                MessageBoxImage.Warning,
                MessageBoxButton.OKCancel,
                "Confirm");

            Messenger.Raise(message);
            if (message.Response != true) { return; }

            CanClose = true;
            DispatcherHelper.UIDispatcher.BeginInvoke((Action)(() =>
            {
                Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction"));
            }));
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

            Hotkeys = null;
            base.Dispose(disposing);
        }
    }
}
