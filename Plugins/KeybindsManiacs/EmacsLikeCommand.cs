using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    class EmacsLikeCommand
    {
        public static void Yank(StateController stateController, KeybindsManiacsSettings settings)
        {
            KeybindsManiacs.UIAssistantAPI.KeyboardOperation.SendKeys(Key.RightCtrl, Key.V);
            stateController.SwitchMode(settings.Mode, true);
        }

        public static void KillRegion(StateController stateController, KeybindsManiacsSettings settings)
        {
            KeybindsManiacs.UIAssistantAPI.KeyboardOperation.SendKeys(Key.RightCtrl, Key.X);
            stateController.SwitchMode(settings.Mode, true);
        }

        public static void KillRingSave(StateController stateController, KeybindsManiacsSettings settings)
        {
            KeybindsManiacs.UIAssistantAPI.KeyboardOperation.SendKeys(Key.RightCtrl, Key.C);
            stateController.SwitchMode(settings.Mode, true);
        }
    }
}
