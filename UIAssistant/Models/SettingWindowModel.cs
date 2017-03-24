using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using UIAssistant.Core.API;
using UIAssistant.Core.Plugin;
using UIAssistant.Core.I18n;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Commands;
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
            var api = UIAssistantAPI.Instance;
            var setting = api.UIAssistantSettings;
            HotkeyResistrant.UnregisterAll();
            foreach (var hotkey in setting.Commands)
            {
                var validationResult = api.CommandAPI.Validate(hotkey.Text);
                if (!validationResult.IsSuccess())
                {
                    api.NotificationAPI.NotifyWarnMessage("Warning", string.Format(TextID.RegisterHotkeyFailed.GetLocalizedText(), validationResult.ErrorMessage));
                    continue;
                }
                var action = PluginManager.Instance.GenerateAction(hotkey.Text);
                try
                {
                    HotkeyResistrant.Register(hotkey.ModifierKeys, hotkey.Key, action);
                }
                catch (DuplicateHotkeyException ex)
                {
                    api.NotificationAPI.NotifyWarnMessage("Warning", string.Format(TextID.HotkeyDuplication.GetLocalizedText(), ex.Message));
                }
            }
        }
    }
}
