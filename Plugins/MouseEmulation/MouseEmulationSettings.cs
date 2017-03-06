using System.IO;
using System.Windows.Input;
using UIAssistant.Interfaces.Settings;

using KeybindHelper;

namespace UIAssistant.Plugin.MouseEmulation
{
    public class MouseEmulationSettings : ISettings
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
        public static readonly string FilePath = Path.Combine(MouseEmulation.UIAssistantAPI.ConfigurationDirectory, FileName);

        public MouseEmulationSettings()
        {
        }

        public void SetValuesDefault()
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

        public static MouseEmulationSettings Load()
        {
            return MouseEmulation.UIAssistantAPI.DefaultSettingsFileIO.Read(typeof(MouseEmulationSettings), FilePath) as MouseEmulationSettings;
        }

        public void Save()
        {
            MouseEmulation.UIAssistantAPI.DefaultSettingsFileIO.Write(typeof(MouseEmulationSettings), this, FilePath);
        }
    }
}
