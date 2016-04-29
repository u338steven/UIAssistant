using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using UIAssistant.Core.Settings;
using UIAssistant.Core.I18n;
using UIAssistant.UI.Controls;
using UIAssistant.Plugin;
using KeybindHelper;

namespace UIAssistant.Models
{
    public class SettingsWindowModel : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */
        public SettingsWindowModel()
        {
        }

        public static void RegisterHotkeys()
        {
            var setting = UserSettings.Instance;
            HotkeyResistrant.UnregisterAll();
            foreach (var hotkey in setting.Commands)
            {
                try
                {
                    HotkeyResistrant.Register(hotkey.ModifierKeys, hotkey.Key, PluginManager.Instance.GenerateAction(hotkey.Text));
                }
                catch (DuplicateHotkeyException ex)
                {
                    Notification.NotifyMessage("Waring", string.Format(TextID.HotkeyDuplication, ex.Message), NotificationIcon.Warning);
                }
            }
        }
    }
}
