using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

using UIAssistant.Infrastructure.Commands;
using System.ComponentModel;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        protected bool SetField<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public KeybindsManiacsSettings Settings { get; private set; } = KeybindsManiacsSettings.Instance;
        public ObservableCollection<string> ModeNames { get; private set; }
        public ObservableCollection<string> EmbeddedCommands { get; private set; }
        public ICandidatesGenerator Generator { get; private set; }
        private bool _IsEnabledWindowsKeybinds;
        public bool IsEnabledWindowsKeybinds { get { return _IsEnabledWindowsKeybinds; } set { SetField(ref _IsEnabledWindowsKeybinds, value); Current.IsEnabledWindowsKeybinds = value; } }
        private bool _IsPrefix;
        public bool IsPrefix { get { return _IsPrefix; } set { SetField(ref _IsPrefix, value); Current.IsPrefix = value; } }
        public ModeStorage Current { get; private set; }

        public SettingsViewModel()
        {
            ModeNames = new ObservableCollection<string>(Settings.KeybindsInMode.Select(x => x.Key));
            Current = Settings.KeybindsInMode[Consts.DefaultMode];
            EmbeddedCommands = new ObservableCollection<string>
            {
                "Exit",
                "Cancel",
                "ChangeMode Normal",
                "ChangeMode Insert",
                "ChangeMode Visual",
                "ChangeMode Emacs",
                "ChangeMode EmacsC-x",
                "ChangeMode Custom1",
                "ChangeMode Custom2",
                "ChangeMode Custom3",
                "ChangeMode Custom4",
                "VimLike f",
                "VimLike F",
                "VimLike t",
                "VimLike T",
                "VimLike ;",
                "VimLike ,",
                "VimLike y",
                "EmacsLike yank",
                "EmacsLike kill-ring-save",
                "EmacsLike kill-region",
                //"EmacsLike set-mark-command",
            };
            Generator = CommandManager.GetGenerator();
        }

        public void SwitchMode(string modeName)
        {
            Current = Settings.KeybindsInMode[modeName];
            IsEnabledWindowsKeybinds = Current.IsEnabledWindowsKeybinds;
            IsPrefix = Current.IsPrefix;
        }

        public void AddNewKeybind()
        {
            Current.Add(new KeyTranslator());
        }
    }
}
