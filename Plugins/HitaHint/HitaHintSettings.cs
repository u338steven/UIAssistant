using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Input;
using UIAssistant.Infrastructure.Settings;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Settings;

using KeybindHelper;

namespace UIAssistant.Plugin.HitaHint
{
    public class HitaHintSettings : Settings<HitaHintSettings>, ISettings
    {
        private string _HintKeys;
        public string HintKeys
        {
            get { return _HintKeys; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception(HitaHint.UIAssistantAPI.Localize("requiredError"));
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
                    throw new Exception(string.Format(HitaHint.UIAssistantAPI.Localize("rangeError"), _minDivisionCount, _maxDivisionCount));
                }
                _screenWidthDivisionCount = value;
            }
        }

        public bool IsMouseCursorHidden { get; set; }

        public string Theme { get; set; } = "General";

        public Keybind SwitchKeyboardLayout { get; } = HitaHint.UIAssistantAPI.UIAssistantSettings.SwitchKeyboardLayout;
        public Keybind SwitchTheme { get; } = HitaHint.UIAssistantAPI.UIAssistantSettings.SwitchTheme;
        public Keybind Up { get; } = HitaHint.UIAssistantAPI.UIAssistantSettings.Up;
        public Keybind Down { get; } = HitaHint.UIAssistantAPI.UIAssistantSettings.Down;
        public Keybind Execute { get; } = HitaHint.UIAssistantAPI.UIAssistantSettings.Execute;
        public Keybind TemporarilyHide { get; } = HitaHint.UIAssistantAPI.UIAssistantSettings.TemporarilyHide;

        public Keybind Quit { get; } = HitaHint.UIAssistantAPI.UIAssistantSettings.Quit;
        public Keybind Back { get; } = HitaHint.UIAssistantAPI.UIAssistantSettings.Back;
        public Keybind Reverse { get; set; } = new Keybind();
        public Keybind Reload { get; set; } = new Keybind();

        public Keybind Click { get; set; } = new Keybind();
        public Keybind RightClick { get; set; } = new Keybind();
        public Keybind MiddleClick { get; set; } = new Keybind();
        public Keybind DoubleClick { get; set; } = new Keybind();
        public Keybind Hover { get; set; } = new Keybind();
        public Keybind DragAndDrop { get; set; } = new Keybind();

        public Keybind MouseEmulation { get; set; } = new Keybind();

        //public readonly UserSettings _userSettings = HitaHint.UIAssistantAPI.UIAssistantSettings;

        private const string FileName = "HitaHint.yml";
        private IFileIO<HitaHintSettings> _fileIO = new YamlFile<HitaHintSettings>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configurations", FileName));
        protected override IFileIO<HitaHintSettings> FileIO { get { return _fileIO; } }

        public HitaHintSettings()
        {
            OnError = ex => HitaHint.UIAssistantAPI.NotifyWarnMessage("Load Settings Error", string.Format(HitaHint.UIAssistantAPI.Localize(TextID.SettingsLoadError), FileName));
        }

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
