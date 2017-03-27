using System.Windows.Controls;
using KeybindHelper.LowLevel;
using UIAssistant.Interfaces.Input;

namespace UIAssistant.Core.Input
{
    public class KeyInputContext : IKeyboardPluginContext
    {
        public IHookHandlers HookHandlers { get; private set; }
        public IKeybindManager Keybinds { get; private set; }
        public UserControl UsagePanel { get; set; }

        public KeyInputContext(IHookHandlers handlers, IKeybindManager keybinds)
        {
            HookHandlers = handlers;
            Keybinds = keybinds;
        }

        public void Dispose()
        {
            Keybinds.Clear();
            HookHandlers.Dispose();
        }
    }
}
