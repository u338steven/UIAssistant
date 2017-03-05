using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeybindHelper.LowLevel;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.API;
using UIAssistant.Infrastructure.Logger;

namespace UIAssistant.Plugin.HitaHint
{
    internal class KeyInputController : AbstractKeyInputController
    {
        private StateController _stateController;
        private HitaHintSettings _settings;

        public KeyInputController(IUIAssistantAPI api, StateController controller) : base(api, controller, api.CreateKeyboardHook(), api.CreateKeybindManager())
        {
            _stateController = controller;
            _settings = _stateController.Settings;
            InitializeKeybind();
        }

        public override void Initialize()
        {
            base.Initialize();
            _stateController.SetKeyboardLayoutName(Hook.GetKeyboardLayoutLanguage());
            Hook.KeyDown += Hook_KeyDown;
        }

        private void Hook_KeyDown(object sender, LowLevelKeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;

            try
            {
                var keysState = e.PressedKeys;
                var input = e.ConvertToCurrentLanguage();
                if (Keybinds.Contains(keysState))
                {
                    Keybinds[keysState]?.Invoke();
                    _stateController.PrintState();
                    return;
                }

                if (input.Length > 0)
                {
                    _stateController.FilterHints(input);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public override void Reset()
        {
            base.Reset();
        }

        protected override void InitializeKeybind()
        {
            UIAssistantAPI.UIDispatcher.Invoke(() => UsagePanel = new Usage());
            Keybinds.Clear();
            Keybinds.Add(UIAssistantAPI.UIAssistantSettings.Quit, () =>
            {
                _stateController.ActivateLastActiveWindow();
                _stateController.Quit();
            });
            Keybinds.Add(UIAssistantAPI.UIAssistantSettings.Back, () => _stateController.Back());
            Keybinds.Add(_settings.Reload, () =>
            {
                _stateController.Clear();
                _stateController.Enumerate();
                _stateController.PrintState();
            });
            Keybinds.Add(_settings.Reverse, () => UIAssistantAPI.DefaultHUD.Items = UIAssistantAPI.DefaultHUD.Items.Reverse().ToList());

            Keybinds.Add(_settings.Click, () => _stateController.ChangeOperation(Consts.Click));
            Keybinds.Add(_settings.RightClick, () => _stateController.ChangeOperation(Consts.RightClick));
            Keybinds.Add(_settings.MiddleClick, () => _stateController.ChangeOperation(Consts.MiddleClick));
            Keybinds.Add(_settings.DoubleClick, () => _stateController.ChangeOperation(Consts.DoubleClick));
            Keybinds.Add(_settings.Hover, () => _stateController.ChangeOperation(Consts.Hover));
            Keybinds.Add(_settings.DragAndDrop, () => _stateController.ChangeOperation(Consts.DragAndDrop));

            Keybinds.Add(_settings.MouseEmulation, () => _stateController.ChangeOperation(Consts.MouseEmulation));
            Keybinds.Add(UIAssistantAPI.UIAssistantSettings.SwitchKeyboardLayout, () =>
            {
                Hook.LoadAnotherKeyboardLayout();
                var layoutLanguage = Hook.GetKeyboardLayoutLanguage();
                UIAssistantAPI.NotifyInfoMessage("Switch Keyboad Layout", string.Format(UIAssistantAPI.Localize(TextID.SwitchKeyboardLayout), layoutLanguage));
                _stateController.SetKeyboardLayoutName(layoutLanguage);
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
                    _stateController.Invoke(UIAssistantAPI.DefaultHUD.Items.ElementAt(0));
                }

            });
            base.InitializeKeybind();
        }
    }
}
