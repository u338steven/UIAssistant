using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Session;

namespace UIAssistant.Interfaces.API
{
    public interface IKeyboardAPI
    {
        IKeyboardOperation KeyboardOperation { get; }

        IKeybindManager CreateKeybindManager();
        IKeyInputController CreateKeyboardController(IKeyboardPlugin plugin, ISession session);
        IKeyboardHook CreateKeyboardHook();
    }
}