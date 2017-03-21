using UIAssistant.Core.Input;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Session;

namespace UIAssistant.Core.API
{
    class KeyboardAPI : IKeyboardAPI
    {
        public IKeyboardOperation KeyboardOperation { get; } = new KeyboardOperation();

        public IKeyboardHook CreateKeyboardHook()
        {
            return new KeyboardHook();
        }

        public IKeybindManager CreateKeybindManager()
        {
            return new KeybindManager();
        }

        public IKeyInputController CreateKeyboardController(IKeyboardPlugin plugin, ISession session)
        {
            var controller = new KeyInputController(plugin, session);
            controller.Initialize();
            //controller.Observe();
            return controller;
        }
    }
}
