using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using UIAssistant.Core.Settings;
using UIAssistant.Infrastructure.Settings;

using KeybindHelper;

namespace UIAssistant.Plugin.HitaHint
{
    public class HitaHintSettings : Settings<HitaHintSettings>
    {
        private string _HintKeys;
        public string HintKeys
        {
            get { return _HintKeys; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception(UIAssistantAPI.Localize("requiredError"));
                }
                _HintKeys = value;
            }
        }

        private const int _minDivisionCount = 1;
        private const int _maxDivisionCount = 32;
        private int _screenWidthDivisionCount;
        public int ScreenWidthDivisionCount
        {
            get { return _screenWidthDivisionCount; }
            set
            {
                if (value < _minDivisionCount || value > _maxDivisionCount)
                {
                    throw new Exception(string.Format(UIAssistantAPI.Localize("rangeError"), _minDivisionCount, _maxDivisionCount));
                }
                _screenWidthDivisionCount = value;
            }
        }

        public bool IsMouseCursorHidden { get; set; }

        public string Theme { get; set; } = "General";

        public Keybind SwitchKeyboardLayout { get; } = UserSettings.Instance.SwitchKeyboardLayout;
        public Keybind SwitchTheme { get; } = UserSettings.Instance.SwitchTheme;
        public Keybind Up { get; } = UserSettings.Instance.Up;
        public Keybind Down { get; } = UserSettings.Instance.Down;
        public Keybind Execute { get; } = UserSettings.Instance.Execute;
        public Keybind TemporarilyHide { get; } = UserSettings.Instance.TemporarilyHide;

        public Keybind Quit { get; } = UserSettings.Instance.Quit;
        public Keybind Back { get; } = UserSettings.Instance.Back;
        public Keybind Reverse { get; set; } = new Keybind();
        public Keybind Reload { get; set; } = new Keybind();

        public Keybind Click { get; set; } = new Keybind();
        public Keybind RightClick { get; set; } = new Keybind();
        public Keybind MiddleClick { get; set; } = new Keybind();
        public Keybind DoubleClick { get; set; } = new Keybind();
        public Keybind Hover { get; set; } = new Keybind();
        public Keybind DragAndDrop { get; set; } = new Keybind();

        public Keybind MouseEmulation { get; set; } = new Keybind();

        public readonly UserSettings _userSettings = UserSettings.Instance;

        private IFileIO<HitaHintSettings> _fileIO = new YamlFile<HitaHintSettings>(UIAssistantDirectory.Configurations, "HitaHint.yml");
        protected override IFileIO<HitaHintSettings> FileIO { get { return _fileIO; } }

        protected override void LoadDefault()
        {
            HintKeys = "asdfghjkl";
            ScreenWidthDivisionCount = 9;
            Theme = "General";

            Reverse = new Keybind(Key.R, LRExtendedModifierKeys.LShift);
            Reload = new Keybind(Key.R, LRExtendedModifierKeys.LControl);

            Click = new Keybind(Key.C);
            RightClick = new Keybind(Key.R);
            MiddleClick = new Keybind(Key.M);
            DoubleClick = new Keybind(Key.B);
            Hover = new Keybind(Key.V);
            DragAndDrop = new Keybind(Key.P);

            MouseEmulation = new Keybind(Key.O);
        }
    }
}
