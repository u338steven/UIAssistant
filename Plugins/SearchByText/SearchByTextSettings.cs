using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using UIAssistant.Core.I18n;
using UIAssistant.Core.Settings;
using UIAssistant.Infrastructure.Settings;
using UIAssistant.Interfaces.Settings;
using UIAssistant.UI.Controls;

using KeybindHelper;

namespace UIAssistant.Plugin.SearchByText
{
    public class SearchByTextSettings : Settings<SearchByTextSettings>
    {
        public Keybind Left { get; } = UserSettings.Instance.Left;
        public Keybind Right { get; } = UserSettings.Instance.Right;
        public Keybind Up { get; } = UserSettings.Instance.Up;
        public Keybind Down { get; } = UserSettings.Instance.Down;
        public Keybind PageUp { get; } = UserSettings.Instance.PageUp;
        public Keybind PageDown { get; } = UserSettings.Instance.PageDown;
        public Keybind Home { get; } = UserSettings.Instance.Home;
        public Keybind End { get; } = UserSettings.Instance.End;
        public Keybind Execute { get; } = UserSettings.Instance.Execute;

        public Keybind Quit { get; } = UserSettings.Instance.Quit;
        public Keybind Back { get; } = UserSettings.Instance.Back;
        public Keybind Delete { get; } = UserSettings.Instance.Delete;
        public Keybind Clear { get; } = UserSettings.Instance.Clear;

        public Keybind SwitchKeyboardLayout { get; } = UserSettings.Instance.SwitchKeyboardLayout;
        public Keybind SwitchTheme { get; } = UserSettings.Instance.SwitchTheme;

        public Keybind Expand { get; set; } = new Keybind();

        public readonly UserSettings _userSettings = UserSettings.Instance;

        private const string FileName = "SearchByText.yml";
        private IFileIO<SearchByTextSettings> _fileIO = new YamlFile<SearchByTextSettings>(UIAssistantDirectory.Configurations, FileName);
        protected override IFileIO<SearchByTextSettings> FileIO { get { return _fileIO; } }

        public SearchByTextSettings()
        {
            OnError = ex => Notification.NotifyMessage("Load Settings Error", string.Format(TextID.SettingsLoadError.GetLocalizedText(), FileName), NotificationIcon.Warning);
        }

        protected override void LoadDefault()
        {
            Expand = new Keybind(Key.Space, LRExtendedModifierKeys.LShift);
        }
    }
}
