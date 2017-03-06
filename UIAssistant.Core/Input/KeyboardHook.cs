using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

using KeybindHelper.LowLevel;
using UIAssistant.Core.API;
using UIAssistant.Core.I18n;
using UIAssistant.Core.Settings;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Input;
using UIAssistant.UI.Controls;

namespace UIAssistant.Core.Input
{
    public class KeyboardHook : LowLevelKeyHook, IKeyboardHook
    {
        private KeySet _terminate;
        public KeyboardHook()
        {
            _terminate = new KeySet(UIAssistantAPI.Instance.UIAssistantSettings.EmergencySwitch);
            PreviewKeyDown += _hook_PreviewKeyDown;
        }

        private void _hook_PreviewKeyDown(object sender, LowLevelKeyEventArgs e)
        {
            if (e.CurrentKey.Key == Key.None || e.CurrentKey.Key == Key.PrintScreen)
            {
                e.Handled = true;
                return;
            }

            if (Contains(e.PressedKeys))
            {
                Notification.NotifyMessage("Forced termination", TextID.ForcedTermination.GetLocalizedText(), NotificationIcon.Warning);
                var t = Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(3000);
                    System.Windows.Application.Current.Dispatcher.Invoke(() => System.Windows.Application.Current.Shutdown());
                });
                IsActive = false;
                e.Handled = true;
                return;
            }
        }

        private bool Contains(KeySet keysState)
        {
            return _terminate.Keys.All(x => keysState.Keys.Contains(x));
        }
    }
}
