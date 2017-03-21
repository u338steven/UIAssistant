using System.Windows.Controls;
using UIAssistant.Interfaces.Input;

namespace UIAssistant.Core.Input
{
    public class KeyInputContext : IKeyboardPluginContext
    {
        public IKeyboardHook Hook { get; private set; }
        public IKeybindManager Keybinds { get; private set; }
        public UserControl UsagePanel { get; set; }

        public KeyInputContext(IKeyboardHook hook, IKeybindManager keybinds)
        {
            Hook = hook;
            Keybinds = keybinds;
        }

        public void Dispose()
        {
            Keybinds.Clear();
            Hook.Dispose();
        }
    }
}
