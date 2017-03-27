using System.Windows.Input;
using KeybindHelper.LowLevel;
using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Session;

namespace UIAssistant.Interfaces.API
{
    public interface IKeyboardAPI
    {
        IKeyboardOperation KeyboardOperation { get; }
        string KeyboardLayoutLanguage { get; }

        IKeybindManager CreateKeybindManager();
        IKeyInputController CreateKeyboardController(IKeyboardPlugin plugin, ISession session);
        IHookHandlers CreateHookHandlers();
        void LoadAnotherKeyboardLayout();
        bool IsPressed(Key key);
        void Hook(IHookHandlers handlers);
        void Unhook(IHookHandlers handlers);
    }
}