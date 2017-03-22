using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using UIAssistant.Core.API;
using UIAssistant.Core.Plugin;
using UIAssistant.Core.Settings;
using UIAssistant.Core.I18n;
using UIAssistant.Infrastructure.Commands;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Commands;
using UIAssistant.UI.Controls;
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
            var setting = UIAssistantAPI.Instance.UIAssistantSettings;
            HotkeyResistrant.UnregisterAll();
            foreach (var hotkey in setting.Commands)
            {
                var validationResult = CommandManager.GetValidator(DefaultLocalizer.Instance).Validate(hotkey.Text);
                if (!validationResult.IsSuccess())
                {
                    UIAssistantAPI.Instance.NotificationAPI.NotifyWarnMessage("Warning", string.Format(TextID.RegisterHotkeyFailed.GetLocalizedText(), validationResult.ErrorMessage));
                    continue;
                }
                var action = PluginManager.Instance.GenerateAction(hotkey.Text);
                try
                {
                    HotkeyResistrant.Register(hotkey.ModifierKeys, hotkey.Key, action);
                }
                catch (DuplicateHotkeyException ex)
                {
                    UIAssistantAPI.Instance.NotificationAPI.NotifyWarnMessage("Warning", string.Format(TextID.HotkeyDuplication.GetLocalizedText(), ex.Message));
                }
            }
        }
    }
}
