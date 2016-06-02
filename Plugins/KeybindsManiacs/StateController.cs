using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    class StateController : AbstractStateController
    {
        public KeybindsManiacsSettings Settings { get; private set; }
        public Mode Mode { get; private set; }

        public StateController()
        {
            Settings = KeybindsManiacsSettings.Instance;
            Mode = Mode.Normal;
        }

        public override void Quit()
        {
            Mode = Mode.Normal;
            Cleanup();
        }

        public override void SwitchNextTheme()
        {
        }

        public void SwitchMode(string modeName, bool isSilent = false)
        {
            Mode = Mode.UserCustom;
            if (!isSilent)
            {
                //UIAssistantAPI.NotifyInfoMessage("KeybindsManiacs", string.Format(KeybindsManiacs.Localizer.GetLocalizedText(Consts.SwitchMode), modeName));
            }
        }

        public void SwitchMode(Mode mode, bool isSilent = false)
        {
            if (Mode == mode)
            {
                return;
            }
            Mode = mode;
            if (!isSilent)
            {
                //UIAssistantAPI.NotifyInfoMessage("KeybindsManiacs", string.Format(KeybindsManiacs.Localizer.GetLocalizedText(Consts.SwitchMode), mode.ToString()));
            }
        }
    }
}
