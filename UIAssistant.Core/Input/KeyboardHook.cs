using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

using KeybindHelper.LowLevel;
using UIAssistant.Core.Settings;
using UIAssistant.Core.I18n;
using UIAssistant.UI.Controls;

namespace UIAssistant.Core.Input
{
    public class KeyboardHook : LowLevelKeyHook
    {
        public static event EventHandler ForceClosing;

        private KeySet _terminate;
        public KeyboardHook()
        {
            _terminate = new KeySet(UserSettings.Instance.EmergencySwitch);
            PreviewKeyPressCallback += KeyInput_PreviewKeyPressCallback;
            AddThroughKeys(Key.PrintScreen);
        }

        private bool Contains(KeySet keysState)
        {
            return _terminate.Keys.All(x => keysState.Keys.Contains(x));
        }

        private bool KeyInput_PreviewKeyPressCallback(KeyEvent keyEvent, Key key, KeySet keysState)
        {
            if (key == Key.None)
            {
                return true;
            }

            if (Contains(keysState))
            {
                Notification.NotifyMessage("Forced termination", TextID.ForcedTermination.GetLocalizedText(), NotificationIcon.Warning);
                var t = Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(3000);
                    if (ForceClosing != null)
                        Task.Factory.FromAsync((asyncCallback, obj) => ForceClosing.BeginInvoke(this, EventArgs.Empty, asyncCallback, obj), ForceClosing.EndInvoke, null)
                            .ToObservable()
                            .Timeout(TimeSpan.FromSeconds(3));
                    System.Windows.Application.Current.Dispatcher.Invoke(() => System.Windows.Application.Current.Shutdown());
                });
                IsActive = false;
                return true;
            }
            return false;
        }
    }
}
