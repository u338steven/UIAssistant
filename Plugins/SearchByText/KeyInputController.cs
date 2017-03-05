using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;

using UIAssistant.Core.I18n;
using UIAssistant.Core.Input;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Settings;
using UIAssistant.Utility.Extensions;
using KeybindHelper.LowLevel;

namespace UIAssistant.Plugin.SearchByText
{
    internal class KeyInputController : AbstractKeyInputController
    {
        private StateController _stateController;
        private IUserSettings _settings;
        private CompositeDisposable _disposables = new CompositeDisposable();

        public KeyInputController(IUIAssistantAPI api, StateController controller) : base(api, controller, new KeyboardHook(), new KeybindManager())
        {
            _stateController = controller;
            _settings = _stateController.Settings;
            InitializeKeybind();
        }

        public override void Initialize()
        {
            base.Initialize();
            Hook.IgnoreInjected = true;

            ObserveEvent(SearchByText.UIAssistantAPI.DefaultHUD.TextBox);
            ObserveEvent(SearchByText.UIAssistantAPI.DefaultContextHUD.TextBox);
            Hook.KeyDown += Hook_KeyDown;
        }

        private void ObserveEvent(ITextBox textBox)
        {
            Observable.FromEvent<TextChangedEventHandler, TextChangedEventArgs>(
                action => (s, ev) => action(ev),
                h => textBox.TextChanged += h,
                h => textBox.TextChanged -= h)
                .Where(_ => Hook.IsActive)
                .Throttle(TimeSpan.FromMilliseconds(150))
                .Subscribe(ev => _stateController.Filter())
                .AddTo(_disposables);
        }

        private void Hook_KeyDown(object sender, LowLevelKeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;

            var keysState = e.PressedKeys;
            var input = e.ConvertToCurrentLanguage();
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
        }

        public override void Reset()
        {
            base.Reset();
        }

        protected override void InitializeKeybind()
        {
            SearchByText.UIAssistantAPI.UIDispatcher.Invoke(() => UsagePanel = new Usage());
            Keybinds.Clear();
            Keybinds.Add(_settings.Quit, () => { _stateController.Cancel(); _stateController.Quit(); });
            Keybinds.Add(_settings.Back, () => SearchByText.UIAssistantAPI.CurrentHUD.TextBox.Backspace());
            Keybinds.Add(_settings.Delete, () => SearchByText.UIAssistantAPI.CurrentHUD.TextBox.Delete());
            Keybinds.Add(_settings.Clear, () => SearchByText.UIAssistantAPI.CurrentHUD.TextBox.SetText(""));

            Keybinds.Add(_settings.Left, () => SearchByText.UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToLeft(1));
            Keybinds.Add(_settings.Right, () => SearchByText.UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToRight(1));
            Keybinds.Add(_settings.Up, () => SearchByText.UIAssistantAPI.CurrentHUD.FocusPreviousItem());
            Keybinds.Add(_settings.Down, () => SearchByText.UIAssistantAPI.CurrentHUD.FocusNextItem());
            Keybinds.Add(_settings.PageUp, () => SearchByText.UIAssistantAPI.CurrentHUD.PageUp());
            Keybinds.Add(_settings.PageDown, () => SearchByText.UIAssistantAPI.CurrentHUD.PageDown());
            Keybinds.Add(_settings.Home, () => SearchByText.UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToHead());
            Keybinds.Add(_settings.End, () => SearchByText.UIAssistantAPI.CurrentHUD.TextBox.MoveCaretToTail());
            Keybinds.Add(_settings.Execute, () => _stateController.Execute());
            Keybinds.Add(_settings.ShowExtraActions, () => _stateController.SwitchHUD());
            Keybinds.Add(SearchByTextSettings.Instance.Expand, () =>
            {
                _stateController.Expand();
                _disposables.Clear();
                ObserveEvent(SearchByText.UIAssistantAPI.DefaultHUD.TextBox);
                ObserveEvent(SearchByText.UIAssistantAPI.DefaultContextHUD.TextBox);
            });
            Keybinds.Add(_settings.SwitchKeyboardLayout, () =>
            {
                Hook.LoadAnotherKeyboardLayout();
                SearchByText.UIAssistantAPI.NotifyInfoMessage("Switch Keyboad Layout", string.Format(TextID.SwitchKeyboardLayout.GetLocalizedText(), Hook.GetKeyboardLayoutLanguage()));
            });
            base.InitializeKeybind();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _disposables.Dispose();
        }
    }
}
