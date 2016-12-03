using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using UIAssistant.Core.I18n;
using UIAssistant.Core.Settings;
using KeybindHelper.LowLevel;

namespace UIAssistant.Plugin.SearchByText
{
    internal class KeyInputController : AbstractKeyInputController
    {
        private StateController _stateController;
        private UserSettings _settings;

        public KeyInputController(StateController controller) : base(controller)
        {
            _stateController = controller;
            _settings = _stateController.Settings;
            InitializeKeybind();
        }

        public override void Initialize()
        {
            base.Initialize();
            Hook.IgnoreInjected = true;
        }

        public override void Reset()
        {
            base.Reset();
        }

        protected override void InitializeKeybind()
        {
            UIAssistantAPI.UIDispatcher.Invoke(() => UsagePanel = new Usage());
            Keybinds.Clear();
            Keybinds.Add(_settings.Quit, () => { _stateController.Cancel(); _stateController.Quit(); });
            Keybinds.Add(_settings.Back, () => { UIAssistantAPI.CurrentHUD.TextBox.Backspace(); _stateController.Filter(); });
            Keybinds.Add(_settings.Delete, () => { UIAssistantAPI.CurrentHUD.TextBox.Delete(); _stateController.Filter(); });
            Keybinds.Add(_settings.Clear, () => { UIAssistantAPI.CurrentHUD.TextBox.SetText(""); _stateController.Filter(); });

            Keybinds.Add(_settings.Left, () => UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToLeft(1));
            Keybinds.Add(_settings.Right, () => UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToRight(1));
            Keybinds.Add(_settings.Up, () => UIAssistantAPI.CurrentHUD.FocusPreviousItem());
            Keybinds.Add(_settings.Down, () => UIAssistantAPI.CurrentHUD.FocusNextItem());
            Keybinds.Add(_settings.PageUp, () => UIAssistantAPI.CurrentHUD.PageUp());
            Keybinds.Add(_settings.PageDown, () => UIAssistantAPI.CurrentHUD.PageDown());
            Keybinds.Add(_settings.Home, () => UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToHead());
            Keybinds.Add(_settings.End, () => UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToTail());
            Keybinds.Add(_settings.Execute, () => _stateController.Execute());
            Keybinds.Add(_settings.ShowExtraActions, () => _stateController.SwitchHUD());
            Keybinds.Add(SearchByTextSettings.Instance.Expand, () => _stateController.Expand());
            Keybinds.Add(_settings.SwitchKeyboardLayout, () =>
            {
                Hook.LoadAnotherKeyboardLayout();
                UIAssistantAPI.NotifyInfoMessage("Switch Keyboad Layout", string.Format(TextID.SwitchKeyboardLayout.GetLocalizedText(), Hook.GetKeyboardLayoutLanguage()));
            });
            base.InitializeKeybind();
        }

        protected override void OnKeyDown(KeyEvent keyEvent, Key key, KeySet keysState, ref bool handled)
        {
            var input = keysState.ConvertToCurrentLanguage();
            Task.Run(() =>
            {
                if (Keybinds.Contains(keysState))
                {
                    Keybinds[keysState].Invoke();
                    return;
                }

                if (string.IsNullOrEmpty(input) || char.IsControl(input[0]))
                {
                    return;
                }
                _stateController.Input(input);
            });
        }

        protected override void OnKeyUp(KeyEvent keyEvent, Key key, KeySet keysState, ref bool handled)
        {
        }
    }
}
