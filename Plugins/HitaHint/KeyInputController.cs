using System;
using System.Linq;

using KeybindHelper.LowLevel;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.API;

namespace UIAssistant.Plugin.HitaHint
{
    internal class KeyInputController : IKeyboardPlugin
    {
        HitaHintSettings _settings;
        StateController _stateController;
        IUIAssistantAPI UIAssistantAPI;

        public KeyInputController(IUIAssistantAPI api, StateController controller)
        {
            UIAssistantAPI = api;
            _stateController = controller;
            _settings = HitaHint.Settings;
        }

        public void Initialize(IKeyboardPluginContext context)
        {
            _stateController.State.KeyboardLayoutName = context.Hook.GetKeyboardLayoutLanguage();
        }

        public void Dispose()
        {
            //_keyController.Dispose();
        }

        public void Cleanup(IKeyboardPluginContext context)
        {
            //_keyController.Dispose();
        }

        public void LoadKeybinds(IKeyboardPluginContext context)
        {
            var keybinds = context.Keybinds;
            keybinds.Add(UIAssistantAPI.UIAssistantSettings.Quit, () =>
            {
                _stateController.ActivateLastActiveWindow();
                _stateController.Quit();
            });
            keybinds.Add(UIAssistantAPI.UIAssistantSettings.SwitchTheme, () => _stateController.SwitchNextTheme());
            keybinds.Add(UIAssistantAPI.UIAssistantSettings.Back, () => _stateController.Back());
            keybinds.Add(_settings.Reload, () =>
            {
                if (_stateController.IsBusy)
                {
                    return;
                }
                _stateController.Clear();
                _stateController.Enumerate();
                _stateController.PrintState();
            });
            keybinds.Add(_settings.Reverse, () => UIAssistantAPI.DefaultHUD.Items = UIAssistantAPI.DefaultHUD.Items.Reverse().ToList());

            keybinds.Add(_settings.Click, () => _stateController.ChangeOperation(Consts.Click));
            keybinds.Add(_settings.RightClick, () => _stateController.ChangeOperation(Consts.RightClick));
            keybinds.Add(_settings.MiddleClick, () => _stateController.ChangeOperation(Consts.MiddleClick));
            keybinds.Add(_settings.DoubleClick, () => _stateController.ChangeOperation(Consts.DoubleClick));
            keybinds.Add(_settings.Hover, () => _stateController.ChangeOperation(Consts.Hover));
            keybinds.Add(_settings.DragAndDrop, () => _stateController.ChangeOperation(Consts.DragAndDrop));

            keybinds.Add(_settings.MouseEmulation, () => _stateController.ChangeOperation(Consts.MouseEmulation));
            keybinds.Add(UIAssistantAPI.UIAssistantSettings.SwitchKeyboardLayout, () =>
            {
                context.Hook.LoadAnotherKeyboardLayout();
                var layoutLanguage = context.Hook.GetKeyboardLayoutLanguage();
                UIAssistantAPI.NotificationAPI.NotifyInfoMessage("Switch Keyboad Layout", string.Format(UIAssistantAPI.Localize(TextID.SwitchKeyboardLayout), layoutLanguage));
                _stateController.State.KeyboardLayoutName = layoutLanguage;
            });

            keybinds.Add(UIAssistantAPI.UIAssistantSettings.Up, () => UIAssistantAPI.DefaultHUD.FocusPreviousItem());
            keybinds.Add(UIAssistantAPI.UIAssistantSettings.Down, () => UIAssistantAPI.DefaultHUD.FocusNextItem());
            keybinds.Add(UIAssistantAPI.UIAssistantSettings.Execute, () =>
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
        }

        public void OnKeyDown(IKeyboardPluginContext context, object sender, LowLevelKeyEventArgs e)
        {
            e.Handled = true;

            try
            {
                var keysState = e.PressedKeys;
                var input = e.ConvertToCurrentLanguage();
                if (context.Keybinds.Contains(keysState))
                {
                    context.Keybinds[keysState]?.Invoke();
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
                UIAssistantAPI.PrintErrorMessage(ex);
            }
        }

        public void OnKeyUp(IKeyboardPluginContext context, object sender, LowLevelKeyEventArgs e)
        {
        }
    }
}
