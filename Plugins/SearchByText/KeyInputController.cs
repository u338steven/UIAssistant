using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;

using UIAssistant.Interfaces;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Settings;
using KeybindHelper.LowLevel;

namespace UIAssistant.Plugin.SearchByText
{
    internal class KeyInputController : IKeyboardPlugin
    {
        private StateController _stateController;
        private IUserSettings _settings;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private IUIAssistantAPI UIAssistantAPI;

        public KeyInputController(IUIAssistantAPI api, StateController controller)
        {
            _stateController = controller;
            _settings = _stateController.Settings;
            UIAssistantAPI = api;
        }

        public void Initialize(IKeyboardPluginContext context)
        {
            context.Hook.IgnoreInjected = true;
            ObserveEvent(UIAssistantAPI.DefaultHUD.TextBox);
            ObserveEvent(UIAssistantAPI.DefaultContextHUD.TextBox);
        }

        private void ObserveEvent(ITextBox textBox)
        {
            Observable.FromEvent<TextChangedEventHandler, TextChangedEventArgs>(
                action => (s, ev) => action(ev),
                h => textBox.TextChanged += h,
                h => textBox.TextChanged -= h)
                .Throttle(TimeSpan.FromMilliseconds(150))
                .Subscribe(ev => _stateController.Filter())
                .AddTo(_disposables);
        }

        public void OnKeyDown(IKeyboardPluginContext context, object sender, LowLevelKeyEventArgs e)
        {
            e.Handled = true;

            var keysState = e.PressedKeys;
            var input = e.ConvertToCurrentLanguage();
            if (context.Keybinds.Contains(keysState))
            {
                context.Keybinds[keysState].Invoke();
                return;
            }

            if (string.IsNullOrEmpty(input) || char.IsControl(input[0]))
            {
                return;
            }
            _stateController.Input(input);
        }

        public void OnKeyUp(IKeyboardPluginContext context, object sender, LowLevelKeyEventArgs e)
        {
        }

        public void LoadKeybinds(IKeyboardPluginContext context)
        {
            var keybinds = context.Keybinds;
            keybinds.Clear();
            keybinds.Add(_settings.Quit, () => { _stateController.Cancel(); _stateController.Quit(); });
            keybinds.Add(_settings.Back, () => UIAssistantAPI.CurrentHUD.TextBox.Backspace());
            keybinds.Add(_settings.Delete, () => UIAssistantAPI.CurrentHUD.TextBox.Delete());
            keybinds.Add(_settings.Clear, () => UIAssistantAPI.CurrentHUD.TextBox.SetText(""));

            keybinds.Add(_settings.Left, () => UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToLeft(1));
            keybinds.Add(_settings.Right, () => UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToRight(1));
            keybinds.Add(_settings.Up, () => UIAssistantAPI.CurrentHUD.FocusPreviousItem());
            keybinds.Add(_settings.Down, () => UIAssistantAPI.CurrentHUD.FocusNextItem());
            keybinds.Add(_settings.PageUp, () => UIAssistantAPI.CurrentHUD.PageUp());
            keybinds.Add(_settings.PageDown, () => UIAssistantAPI.CurrentHUD.PageDown());
            keybinds.Add(_settings.Home, () => UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToHead());
            keybinds.Add(_settings.End, () => UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToTail());
            keybinds.Add(_settings.Execute, () => _stateController.Execute());
            keybinds.Add(_settings.ShowExtraActions, () => _stateController.SwitchHUD());
            keybinds.Add(SearchByText.Settings.Expand, () =>
            {
                _stateController.Expand();
                _disposables.Clear();
                ObserveEvent(UIAssistantAPI.DefaultHUD.TextBox);
                ObserveEvent(UIAssistantAPI.DefaultContextHUD.TextBox);
            });
            keybinds.Add(_settings.SwitchTheme, () => _stateController.SwitchNextTheme());
            keybinds.Add(_settings.SwitchKeyboardLayout, () =>
            {
                context.Hook.LoadAnotherKeyboardLayout();
                UIAssistantAPI.NotificationAPI.NotifyInfoMessage("Switch Keyboad Layout", string.Format(UIAssistantAPI.Localize(TextID.SwitchKeyboardLayout), context.Hook.GetKeyboardLayoutLanguage()));
            });
        }

        public void Cleanup(IKeyboardPluginContext context)
        {
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
