using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

using KeybindHelper.LowLevel;
using UIAssistant.Core.Input;

namespace UIAssistant.Plugin
{
    public abstract class AbstractKeyInputController : IDisposable
    {
        protected KeyboardHook Hook { get; set; }
        protected AbstractStateController StateController { get; set; }
        protected KeybindManager Keybinds { get; set; } = new KeybindManager();
        private KeySet _temporarilyHide = new KeySet();
        protected UserControl UsagePanel;

        public AbstractKeyInputController(AbstractStateController controller)
        {
            StateController = controller;
            StateController.Pausing += (_, __) => Hook.IsActive = true;
            StateController.Resumed += (_, __) => Hook.IsActive = false;
        }

        public virtual void Reset()
        {
            InitializeKeybind();
        }

        public virtual void Initialize()
        {
            Hook = new KeyboardHook();
            Hook.Hook();
            Hook.HookedKeyboardCallback += hookedKeyboardCallback;

            StateController.Finished += (_, __) =>
            {
                Hook.HookedKeyboardCallback -= hookedKeyboardCallback;
                Hook.Dispose();
            };
        }

        protected virtual void InitializeKeybind()
        {
            Keybinds.Add(UIAssistantAPI.UIAssistantSettings.SwitchTheme, () => StateController.SwitchNextTheme());
            Keybinds.Add(UIAssistantAPI.UIAssistantSettings.Usage, () =>
            {
                if (UsagePanel == null)
                {
                    return;
                }
                if (!UsagePanel.IsVisible)
                {
                    UIAssistantAPI.AddPanel(UsagePanel);
                    StateController.Finished += RemoveUsagePanel;
                }
                else
                {
                    RemoveUsagePanel(this, EventArgs.Empty);
                    StateController.Finished -= RemoveUsagePanel;
                }
            });
            _temporarilyHide = new KeySet(UIAssistantAPI.UIAssistantSettings.TemporarilyHide);
        }

        private bool hookedKeyboardCallback(KeyEvent keyEvent, Key key, KeySet keysState)
        {
            bool handled = true;
            switch (keyEvent)
            {
                case KeyEvent.WM_KEYDOWN:
                case KeyEvent.WM_SYSKEYDOWN:
#if DEBUG
                    if (!keysState.IsInjected)
                    {
                        UIAssistantAPI.DisplayKeystroke(key, keysState);
                    }
                    //System.Diagnostics.Debug.Print($"key:{keysState.ToString()}, {input}, {input.ToUpper()}");
#endif
                    if (keysState.Equals(_temporarilyHide))
                    {
                        UIAssistantAPI.Transparent = true;
                        return true;
                    }

                    OnKeyDown(keyEvent, key, keysState, ref handled);
                    return handled;
                case KeyEvent.WM_KEYUP:
                case KeyEvent.WM_SYSKEYUP:
                    if (UIAssistantAPI.Transparent)
                    {
                        UIAssistantAPI.Transparent = false;
                        return true;
                    }
                    OnKeyUp(keyEvent, key, keysState, ref handled);
                    return handled;
                default:
                    break;
            }
            return false;
        }

        protected abstract void OnKeyDown(KeyEvent keyEvent, Key key, KeySet keysState, ref bool handled);
        protected abstract void OnKeyUp(KeyEvent keyEvent, Key key, KeySet keysState, ref bool handled);

        private void RemoveUsagePanel(object sender, EventArgs e)
        {
            UIAssistantAPI.RemovePanel(UsagePanel);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Hook != null)
                    {
                        Hook.HookedKeyboardCallback -= hookedKeyboardCallback;
                        Hook.Dispose();
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
