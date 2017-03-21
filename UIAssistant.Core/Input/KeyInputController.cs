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
            _context = new KeyInputContext(UIAssistantAPI.Instance.CreateKeyboardHook(), UIAssistantAPI.Instance.CreateKeybindManager());

            _session = session;
            _session.Pausing += (_, __) => _context.Hook.IsActive = false;
            _session.Resumed += (_, __) => _context.Hook.IsActive = true;
        }

        public void AddHidingProcess()
        {
            _temporarilyHide = new KeySet(UIAssistantAPI.Instance.UIAssistantSettings.TemporarilyHide);
            _context.Hook.KeyDown += Hide_KeyDown;
            _context.Hook.KeyUp += Hide_KeyUp;
        }

        private void Hide_KeyDown(object sender, LowLevelKeyEventArgs e)
        {
            if (e.PressedKeys.Equals(_temporarilyHide))
            {
                UIAssistantAPI.Instance.Transparent = true;
                e.Handled = true;
                return;
            }
        }

        private void Hide_KeyUp(object sender, LowLevelKeyEventArgs e)
        {
            if (UIAssistantAPI.Instance.Transparent)
            {
                UIAssistantAPI.Instance.Transparent = false;
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
                    UIAssistantAPI.Instance.AddPanel(_context.UsagePanel);
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
            if (_context.Hook.IsActive)
            {
                return;
            }

            _context.Hook.Hook();
            _context.Hook.KeyDown += CallPluginKeyDown;
            _context.Hook.KeyUp += CallPluginKeyUp;

            _session.Finished += (_, __) =>
            {
                _context.Hook.Unhook();
                _context.Dispose();
            };
        }

        private void CallPluginKeyDown(object sender, LowLevelKeyEventArgs e)
        {
#if DEBUG
            if (!e.CurrentKey.IsInjected)
            {
                UIAssistantAPI.Instance.DisplayKeystroke(e);
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
            UIAssistantAPI.Instance.RemovePanel(_context.UsagePanel);
        }

        public void Dispose()
        {
            _context.Dispose();
            _session.Dispose();
        }
    }
}
