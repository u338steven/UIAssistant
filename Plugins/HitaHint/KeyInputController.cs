using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

using KeybindHelper.LowLevel;
using UIAssistant.Core.Enumerators;
using UIAssistant.Core.I18n;

namespace UIAssistant.Plugin.HitaHint
{
    internal class KeyInputController : AbstractKeyInputController
    {
        private StateController _stateController;
        private HitaHintSettings _settings;

        public KeyInputController(StateController controller) : base(controller)
        {
            _stateController = controller;
            _settings = _stateController.Settings;
            InitializeKeybind();
        }

        public override void Initialize()
        {
            base.Initialize();
            UIAssistantAPI.UIDispatcher.Invoke(() =>
            {
                _stateController.SetKeyboardLayoutName(Hook.GetKeyboardLayoutLanguage());
            });
        }

        public override void Reset()
        {
            base.Reset();
            UIAssistantAPI.UIDispatcher.Invoke(() => UsagePanel = new Usage());
        }

        protected override void InitializeKeybind()
        {
            Keybinds.Clear();
            Keybinds.Add(UIAssistantAPI.UIAssistantSettings.Quit, () =>
            {
                _stateController.ActivateLastActiveWindow();
                _stateController.Quit();
            });
            Keybinds.Add(UIAssistantAPI.UIAssistantSettings.Back, () => _stateController.Back());
            Keybinds.Add(_settings.Reload, () =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    _stateController.Clear();
                    _stateController.Enumerate();
                    _stateController.PrintState();
                }));
            });
            Keybinds.Add(_settings.Reverse, () => UIAssistantAPI.DefaultHUD.Items = new HUDItemCollection(UIAssistantAPI.DefaultHUD.Items.Reverse()));

            Keybinds.Add(_settings.Click, () => _stateController.ChangeOperation(Consts.Click));
            Keybinds.Add(_settings.RightClick, () => _stateController.ChangeOperation(Consts.RightClick));
            Keybinds.Add(_settings.MiddleClick, () => _stateController.ChangeOperation(Consts.MiddleClick));
            Keybinds.Add(_settings.DoubleClick, () => _stateController.ChangeOperation(Consts.DoubleClick));
            Keybinds.Add(_settings.Hover, () => _stateController.ChangeOperation(Consts.Hover));
            Keybinds.Add(_settings.DragAndDrop, () => _stateController.ChangeOperation(Consts.DragAndDrop));

            Keybinds.Add(_settings.MouseEmulation, () => _stateController.ChangeOperation(Consts.MouseEmulation));
            Keybinds.Add(UIAssistantAPI.UIAssistantSettings.SwitchKeyboardLayout, () =>
            {
                UIAssistantAPI.UIDispatcher.Invoke(() =>
                {
                    Hook.LoadAnotherKeyboardLayout();
                    var layoutLanguage = Hook.GetKeyboardLayoutLanguage();
                    UIAssistantAPI.NotifyInfoMessage("Switch Keyboad Layout", string.Format(TextID.SwitchKeyboardLayout.GetLocalizedText(), layoutLanguage));
                    _stateController.SetKeyboardLayoutName(layoutLanguage);
                });
            });

            Keybinds.Add(UIAssistantAPI.UIAssistantSettings.Up, () => UIAssistantAPI.DefaultHUD.FocusPreviousItem());
            Keybinds.Add(UIAssistantAPI.UIAssistantSettings.Down, () => UIAssistantAPI.DefaultHUD.FocusNextItem());
            Keybinds.Add(UIAssistantAPI.UIAssistantSettings.Execute, () =>
            {
                var selectedItem = UIAssistantAPI.DefaultHUD.SelectedItem;
                if (selectedItem != null)
                {
                    _stateController.Invoke(selectedItem);
                }
                else if (UIAssistantAPI.DefaultHUD.Items.Count == 1)
                {
                    _stateController.Invoke(UIAssistantAPI.DefaultHUD.Items[0]);
                }

            });
            base.InitializeKeybind();
        }

        protected override void OnKeyDown(KeyEvent keyEvent, Key key, KeySet keysState)
        {
            var input = keysState.ConvertToCurrentLanguage();
            if (Keybinds.Contains(keysState))
            {
                Task.Run(() =>
                {
                    Keybinds[keysState]?.Invoke();
                    _stateController.PrintState();
                });
                return;
            }

            if (input.Length > 0)
            {
                Task.Run(() => _stateController.FilterHints(input));
            }
        }

        protected override void OnKeyUp(KeyEvent keyEvent, Key key, KeySet keysState)
        {
        }
    }
}
