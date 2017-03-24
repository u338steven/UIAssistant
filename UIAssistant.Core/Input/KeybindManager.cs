using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeybindHelper;
using KeybindHelper.LowLevel;
using UIAssistant.Core.I18n;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Input;
using UIAssistant.UI.Controls;

namespace UIAssistant.Core.Input
{
    public class KeybindManager : IKeybindManager
    {
        private Dictionary<KeySet, Action> _keybinds = new Dictionary<KeySet, Action>();
        private Dictionary<KeySet, bool> _canActWhenKeyHoldDown = new Dictionary<KeySet, bool>();

        public void Add(KeySet keys, Action action, bool canActWhenKeyHoldDown = false)
        {
            if (_keybinds.ContainsKey(keys))
            {
                Notification.NotifyMessage("UIAssistant", string.Format(TextID.KeybindDuplication.GetLocalizedText(), keys.ToString()), NotificationIcon.Warning);
                return;
            }
            _keybinds.Add(keys, action);
            _canActWhenKeyHoldDown.Add(keys, canActWhenKeyHoldDown);
        }

        public void Add(Keybind keybind, Action action, bool canActWhenKeyHoldDown = false)
        {
            var keys = new KeySet(keybind);
            Add(keys, action, canActWhenKeyHoldDown);
        }

        public bool CanActWhenKeyHoldDown(KeySet keys)
        {
            if (_canActWhenKeyHoldDown.ContainsKey(keys))
            {
                return _canActWhenKeyHoldDown[keys];
            }
            return false;
        }

        public bool Contains(KeySet keys)
        {
            return _keybinds.ContainsKey(keys);
        }

        public void Clear()
        {
            _keybinds.Clear();
            _canActWhenKeyHoldDown.Clear();
        }

        public void Execute(KeySet keys, bool isKeyHoldDown)
        {
            if (isKeyHoldDown && !CanActWhenKeyHoldDown(keys))
            {
                return;
            }

            if (_keybinds.ContainsKey(keys))
            {
                _keybinds[keys].Invoke();
            }
        }

        public Action this[KeySet keys]
        {
            get
            {
                if (_keybinds.ContainsKey(keys))
                {
                    return _keybinds[keys];
                }
                return null;
            }
        }
    }
}
