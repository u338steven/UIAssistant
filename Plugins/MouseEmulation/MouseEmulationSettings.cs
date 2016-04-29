using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using UIAssistant.Core.Settings;

using KeybindHelper;

namespace UIAssistant.Plugin.MouseEmulation
{
    public class MouseEmulationSettings : YamlSettings<MouseEmulationSettings>
    {
        public Keybind Quit { get; } = UserSettings.Instance.Quit;
        public Keybind Back { get; } = UserSettings.Instance.Back;

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

        protected override string FileName => @"MouseEmulation.yml";
        protected override MouseEmulationSettings LoadDefault()
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
            return this;
        }
    }
}
