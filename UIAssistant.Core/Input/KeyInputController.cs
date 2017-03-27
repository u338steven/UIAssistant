using System;
using System.Windows.Controls;
using UIAssistant.Core.API;
using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Session;
using KeybindHelper.LowLevel;

namespace UIAssistant.Core.Input
{
    public class KeyInputController : IKeyInputController
    {
        IKeyboardPluginContext _context;
        IKeyboardPlugin _plugin;
        ISession _session;
        private KeySet _temporarilyHide = new KeySet();

        public KeyInputController(IKeyboardPlugin plugin, ISession session)
        {
            _plugin = plugin;
            _context = new KeyInputContext(UIAssistantAPI.Instance.KeyboardAPI.CreateHookHandlers(), UIAssistantAPI.Instance.KeyboardAPI.CreateKeybindManager());

            _session = session;
        }

        public void AddHidingProcess()
        {
            _temporarilyHide = new KeySet(UIAssistantAPI.Instance.UIAssistantSettings.TemporarilyHide);
            _context.HookHandlers.KeyDown += Hide_KeyDown;
            _context.HookHandlers.KeyUp += Hide_KeyUp;
        }

        private void Hide_KeyDown(object sender, LowLevelKeyEventArgs e)
        {
            if (e.PressedKeys.Equals(_temporarilyHide))
            {
                UIAssistantAPI.Instance.ViewAPI.Transparent = true;
                e.Handled = true;
                return;
            }
        }

        private void Hide_KeyUp(object sender, LowLevelKeyEventArgs e)
        {
            if (UIAssistantAPI.Instance.ViewAPI.Transparent)
            {
                UIAssistantAPI.Instance.ViewAPI.Transparent = false;
                e.Handled = true;
                return;
            }
        }

        public void AddUsagePanelProcess(UserControl usagePanel)
        {
            _context.UsagePanel = usagePanel;
            _context.Keybinds.Add(UIAssistantAPI.Instance.UIAssistantSettings.Usage, () =>
            {
                if (_context.UsagePanel == null)
                {
                    return;
                }
                if (!_context.UsagePanel.IsVisible)
                {
                    UIAssistantAPI.Instance.ViewAPI.AddPanel(_context.UsagePanel);
                    _session.Finished += RemoveUsagePanel;
                }
                else
                {
                    RemoveUsagePanel(this, EventArgs.Empty);
                    _session.Finished -= RemoveUsagePanel;
                }
            });
        }

        public void Observe()
        {
            if (_context.HookHandlers.IsActive)
            {
                return;
            }

            _context.HookHandlers.KeyDown += CallPluginKeyDown;
            _context.HookHandlers.KeyUp += CallPluginKeyUp;
            UIAssistantAPI.Instance.KeyboardAPI.Hook(_context.HookHandlers);

            _session.Finished += (_, __) =>
            {
                UIAssistantAPI.Instance.KeyboardAPI.Unhook(_context.HookHandlers);
                _context.Dispose();
            };
        }

        private void CallPluginKeyDown(object sender, LowLevelKeyEventArgs e)
        {
#if DEBUG
            if (!e.CurrentKeyState.IsInjected)
            {
                UIAssistantAPI.Instance.ViewAPI.DisplayKeystroke(e);
            }
#endif
            _plugin.OnKeyDown(_context, sender, e);
        }

        private void CallPluginKeyUp(object sender, LowLevelKeyEventArgs e)
        {
            _plugin.OnKeyUp(_context, sender, e);
        }

        public void Initialize()
        {
            _context.Keybinds.Clear();
            _plugin.Initialize(_context);
            _plugin.LoadKeybinds(_context);
        }

        private void RemoveUsagePanel(object sender, EventArgs e)
        {
            UIAssistantAPI.Instance.ViewAPI.RemovePanel(_context.UsagePanel);
        }

        public void Dispose()
        {
            _context.Dispose();
            _session.Dispose();
        }
    }
}
