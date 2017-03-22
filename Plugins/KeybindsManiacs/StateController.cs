using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Session;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    class StateController
    {
        public KeybindsManiacsSettings Settings { get; private set; }
        public Mode Mode { get; private set; }
        public ISession Session { get; private set; }
        private IUIAssistantAPI API;

        public StateController(IUIAssistantAPI api)
        {
            Settings = KeybindsManiacs.Settings;
            API = api;
        }

        public void Initialize()
        {
            Mode = Mode.Normal;
            Session = API.SessionAPI.Create();
        }

        public void Quit()
        {
            Mode = Mode.Normal;
            Session.Dispose();
        }

        public void SwitchNextTheme()
        {
        }

        public void SwitchMode(string modeName, bool isSilent = false)
        {
            Mode = Mode.UserCustom;
            if (!isSilent)
            {
                API.NotificationAPI.NotifyInfoMessage("KeybindsManiacs", string.Format(KeybindsManiacs.Localizer.GetLocalizedText(Consts.SwitchMode), modeName));
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
                API.NotificationAPI.NotifyInfoMessage("KeybindsManiacs", string.Format(KeybindsManiacs.Localizer.GetLocalizedText(Consts.SwitchMode), mode.ToString()));
            }
        }
    }
}
