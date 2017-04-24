using System.Collections.Generic;
using KeybindHelper.LowLevel;

using UIAssistant.Interfaces.Input;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    class KeybindStorage
    {
        public Dictionary<KeySet, bool> OneShotDefined { get; set; } = new Dictionary<KeySet, bool>();
        public IKeybindManager OneShotKeybinds { get; set; }
        public IKeybindManager Keybinds { get; set; }
        public bool IsEnabledWindowsKeybinds { get; set; } = false;
        public bool IsPrefix { get; set; } = false;

        public KeybindStorage()
        {
            var api = KeybindsManiacs.UIAssistantAPI.KeyboardAPI;
            OneShotKeybinds = api.CreateKeybindManager();
            Keybinds = api.CreateKeybindManager();
        }
    }
}
