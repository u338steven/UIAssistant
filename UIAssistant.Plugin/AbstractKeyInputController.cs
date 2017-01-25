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
            Hook.KeyDown += Hook_KeyDown;
            Hook.KeyUp += Hook_KeyUp;

            StateController.Finished += (_, __) =>
            {
                Hook.Dispose();
            };
        }

        private void Hook_KeyUp(object sender, LowLevelKeyEventArgs e)
        {
            if (UIAssistantAPI.Transparent)
            {
                UIAssistantAPI.Transparent = false;
                e.Handled = true;
                return;
            }
        }

        private void Hook_KeyDown(object sender, LowLevelKeyEventArgs e)
        {
            var keysState = e.PressedKeys;
#if DEBUG
            if (!e.CurrentKey.IsInjected)
            {
                UIAssistantAPI.DisplayKeystroke(e);
            }
            //System.Diagnostics.Debug.Print($"key:{keysState.ToString()}, {input}, {input.ToUpper()}");
#endif
            if (keysState.Equals(_temporarilyHide))
            {
                UIAssistantAPI.Transparent = true;
                e.Handled = true;
                return;
            }
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
