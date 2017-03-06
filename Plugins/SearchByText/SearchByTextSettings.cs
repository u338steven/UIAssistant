using System.IO;
using System.Windows.Input;
using UIAssistant.Interfaces.Settings;

using KeybindHelper;

namespace UIAssistant.Plugin.SearchByText
{
    public class SearchByTextSettings : ISettings
    {
        public Keybind Left { get; }
        public Keybind Right { get; }
        public Keybind Up { get; }
        public Keybind Down { get; }
        public Keybind PageUp { get; }
        public Keybind PageDown { get; }
        public Keybind Home { get; }
        public Keybind End { get; }
        public Keybind Execute { get; }

        public Keybind Quit { get; }
        public Keybind Back { get; }
        public Keybind Delete { get; }
        public Keybind Clear { get; }

        public Keybind SwitchKeyboardLayout { get; }
        public Keybind SwitchTheme { get; }

        public Keybind Expand { get; set; } = new Keybind();

        public readonly IUserSettings _userSettings = SearchByText.UIAssistantAPI.UIAssistantSettings;

        private const string FileName = "SearchByText.yml";
        public static readonly string FilePath = Path.Combine(SearchByText.UIAssistantAPI.ConfigurationDirectory, FileName);

        public SearchByTextSettings()
        {
            Left = _userSettings.Left;
            Right = _userSettings.Right;
            Up = _userSettings.Up;
            Down = _userSettings.Down;
            PageUp = _userSettings.PageUp;
            PageDown = _userSettings.PageDown;
            Home = _userSettings.Home;
            End = _userSettings.End;
            Execute = _userSettings.Execute;

            Quit = _userSettings.Quit;
            Back = _userSettings.Back;
            Delete = _userSettings.Delete;
            Clear = _userSettings.Clear;

            SwitchKeyboardLayout = _userSettings.SwitchKeyboardLayout;
            SwitchTheme = _userSettings.SwitchTheme;
        }

        public void SetValuesDefault()
        {
            Expand = new Keybind(Key.Space, LRExtendedModifierKeys.LShift);
        }

        public static SearchByTextSettings Load()
        {
            return SearchByText.UIAssistantAPI.DefaultSettingsFileIO.Read(typeof(SearchByTextSettings), FilePath) as SearchByTextSettings;
        }

        public void Save()
        {
            SearchByText.UIAssistantAPI.DefaultSettingsFileIO.Write(typeof(SearchByTextSettings), this, FilePath);
        }
    }
}
