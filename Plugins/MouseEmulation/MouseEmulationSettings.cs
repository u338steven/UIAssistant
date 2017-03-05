using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using UIAssistant.Infrastructure.Settings;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Settings;

using KeybindHelper;

namespace UIAssistant.Plugin.MouseEmulation
{
    public class MouseEmulationSettings : Settings<MouseEmulationSettings>
    {
        public Keybind Quit { get; } = MouseEmulation.UIAssistantAPI.UIAssistantSettings.Quit;
        public Keybind Back { get; } = MouseEmulation.UIAssistantAPI.UIAssistantSettings.Back;

        public Keybind Left { get; set; } = new Keybind();
        public Keybind Right { get; set; } = new Keybind();
        public Keybind Up { get; set; } = new Keybind();
        public Keybind Down { get; set; } = new Keybind();

        public Keybind Click { get; set; } = new Keybind();
        public Keybind RightClick { get; set; } = new Keybind();
        public Keybind MiddleClick { get; set; } = new Keybind();

        public Keybind WheelUp { get; set; } = new Keybind();
        public Keybind WheelDown { get; set; } = new Keybind();
        public Keybind HWheelUp { get; set; } = new Keybind();
        public Keybind HWheelDown { get; set; } = new Keybind();

        public Keybind SpeedUp { get; set; } = new Keybind();
        public Keybind SlowDown { get; set; } = new Keybind();

        private const string FileName = "MouseEmulation.yml";
        private IFileIO<MouseEmulationSettings> _fileIO = new YamlFile<MouseEmulationSettings>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configurations", FileName));
        protected override IFileIO<MouseEmulationSettings> FileIO { get { return _fileIO; } }

        public MouseEmulationSettings()
        {
            OnError = ex => MouseEmulation.UIAssistantAPI.NotifyWarnMessage("Load Settings Error", string.Format(MouseEmulation.UIAssistantAPI.Localize(TextID.SettingsLoadError), FileName));
        }

        protected override void LoadDefault()
        {
            Left = new Keybind(Key.J);
            Right = new Keybind(Key.L);
            Up = new Keybind(Key.I);
            Down = new Keybind(Key.K);

            Click = new Keybind(Key.D);
            RightClick = new Keybind(Key.G);
            MiddleClick = new Keybind(Key.A);

            WheelUp = new Keybind(Key.R);
            WheelDown = new Keybind(Key.E);
            HWheelUp = new Keybind(Key.V);
            HWheelDown = new Keybind(Key.C);

            SpeedUp = new Keybind(Key.F);
            SlowDown = new Keybind(Key.S);
        }
    }
}
