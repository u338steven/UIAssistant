using System.Windows.Input;
using KeybindHelper.LowLevel;
using UIAssistant.Core.Input;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Session;

namespace UIAssistant.Core.API
{
    class KeyboardAPI : IKeyboardAPI
    {
        public IKeyboardOperation KeyboardOperation { get; } = new KeyboardOperation();
        private IKeyboard _keyboard { get; } = new LowLevelKeyboard();

        public void Hook(IHookHandlers handlers)
        {
            _keyboard.AddHandlers(handlers);
            _keyboard.Hook();
        }

        public void Unhook(IHookHandlers handlers)
        {
            _keyboard.RemoveHandlers(handlers);
            if (_keyboard.Current == null)
            {
                _keyboard.Unhook();
            }
        }

        public string KeyboardLayoutLanguage
        {
            get
            {
                return _keyboard.GetKeyboardLayoutLanguage();
            }
        }

        public void LoadAnotherKeyboardLayout()
        {
            _keyboard.LoadAnotherKeyboardLayout();
        }

        public bool IsPressed(Key key)
        {
            return _keyboard.IsPressed(key);
        }

        public IHookHandlers CreateHookHandlers()
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
            return controller;
        }
    }
}
