using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeybindHelper;
using KeybindHelper.LowLevel;
using UIAssistant.Core.API;
using UIAssistant.Core.I18n;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Input;

namespace UIAssistant.Core.Input
{
    public class KeybindManager : IKeybindManager
    {
        private Dictionary<KeyState, Dictionary<KeySet, Action>> _keybinds = new Dictionary<KeyState, Dictionary<KeySet, Action>>();
        private Dictionary<KeySet, bool> _canActWhenKeyRepeat = new Dictionary<KeySet, bool>();

        public KeybindManager()
        {
            _keybinds.Add(KeyState.Down, new Dictionary<KeySet, Action>());
            _keybinds.Add(KeyState.Up, new Dictionary<KeySet, Action>());
        }

        public void Add(KeySet keys, Action action, KeyState state = KeyState.Down, bool canActWhenKeyRepeat = false)
        {
            Add(state, keys, action, canActWhenKeyRepeat);
        }

        public void Add(Keybind keybind, Action action, KeyState state = KeyState.Down, bool canActWhenKeyRepeat = false)
        {
            var keys = new KeySet(keybind);
            Add(state, keys, action, canActWhenKeyRepeat);
        }

        public void Add(Keybind keybind, Action keyDown, Action keyUp, bool canActWhenKeyRepeat = false)
        {
            var keys = new KeySet(keybind);
            Add(KeyState.Down, keys, keyDown, canActWhenKeyRepeat);
            Add(KeyState.Up, keys, keyUp, canActWhenKeyRepeat);
        }

        private void Add(KeyState state, KeySet keys, Action action, bool canActWhenKeyRepeat = false)
        {
            if (_keybinds[state].ContainsKey(keys))
            {
                UIAssistantAPI.Instance.NotificationAPI.NotifyWarnMessage("UIAssistant", string.Format(TextID.KeybindDuplication.GetLocalizedText(), keys.ToString()));
                return;
            }
            _keybinds[state].Add(keys, action);
            if (state == KeyState.Down)
            {
                _canActWhenKeyRepeat.Add(keys, canActWhenKeyRepeat);
            }
        }

        public bool CanActWhenKeyRepeat(KeySet keys)
        {
            if (_canActWhenKeyRepeat.ContainsKey(keys))
            {
                return _canActWhenKeyRepeat[keys];
            }
            return false;
        }

        public bool Contains(KeySet keys, KeyState state = KeyState.Down)
        {
            return _keybinds[state].ContainsKey(keys);
        }

        public void Clear()
        {
            _keybinds[KeyState.Down].Clear();
            _keybinds[KeyState.Up].Clear();
            _canActWhenKeyRepeat.Clear();
        }

        public void Execute(KeySet keys, bool isKeyHoldDown = false, KeyState state = KeyState.Down)
        {
            if (isKeyHoldDown && !CanActWhenKeyRepeat(keys))
            {
                return;
            }

            if (_keybinds[state].ContainsKey(keys))
            {
                _keybinds[state][keys].Invoke();
            }
        }

        public Action GetAction(KeySet keys, KeyState state = KeyState.Down)
        {
            if (_keybinds[state].ContainsKey(keys))
            {
                return _keybinds[state][keys];
            }
            return null;
        }
    }
}
