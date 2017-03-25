using System;
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

using UIAssistant.Models;
using UIAssistant.Core.API;
using UIAssistant.Interfaces.Commands;
using UIAssistant.Interfaces.Settings;
using UIAssistant.UI.Controls;

using KeybindHelper;

namespace UIAssistant.ViewModels
{
    public class HotkeyTabViewModel : ViewModel
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

        public void Initialize()
        {
            Settings = UIAssistantAPI.Instance.UIAssistantSettings;
            Hotkeys = new ObservableSynchronizedCollection<Keybind>(Settings.Commands);
            Generator = UIAssistantAPI.Instance.CommandAPI.GetCommandGenerator();
            Validator = UIAssistantAPI.Instance.CommandAPI.GetValidator();
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

        protected override void Dispose(bool disposing)
        {
            Hotkeys = null;
            base.Dispose(disposing);
        }
    }
}
