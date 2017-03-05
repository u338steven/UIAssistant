using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeybindHelper;
using KeybindHelper.LowLevel;
using UIAssistant.Core.I18n;
using UIAssistant.Interfaces.Input;
using UIAssistant.UI.Controls;

namespace UIAssistant.Core.Input
{
    public class KeybindManager : IKeybindManager
    {
        private Dictionary<KeySet, Action> _keybinds = new Dictionary<KeySet, Action>();

        public void Add(KeySet keys, Action action)
        {
            if (_keybinds.ContainsKey(keys))
            {
                Notification.NotifyMessage("UIAssistant", string.Format(TextID.KeybindDuplication.GetLocalizedText(), keys.ToString()), NotificationIcon.Warning);
                return;
            }
            _keybinds.Add(keys, action);
        }

        public void Add(Keybind keybind, Action action)
        {
            var keys = new KeySet(keybind);
            Add(keys, action);
        }

        public bool Contains(KeySet keys)
        {
            return _keybinds.ContainsKey(keys);
        }

        public void Clear()
        {
            _keybinds.Clear();
        }

        public void Execute(KeySet keys)
        {
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
