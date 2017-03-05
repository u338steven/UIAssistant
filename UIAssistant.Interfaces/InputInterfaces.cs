using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using KeybindHelper;
using KeybindHelper.LowLevel;

namespace UIAssistant.Interfaces.Input
{
    public interface IKeyboardHook : IDisposable
    {
        bool IgnoreInjected { get; set; }
        bool IsActive { get; set; }

        event LowLevelKeyEventHandler KeyDown;
        event LowLevelKeyEventHandler KeyUp;
        event LowLevelKeyEventHandler PreviewKeyDown;

        string GetKeyboardLayoutLanguage();
        void Hook();
        bool IsPressed(Key key);
        void LoadAnotherKeyboardLayout();
        void Unhook();
    }

    public interface IKeybindManager
    {
        Action this[KeySet keys] { get; }

        void Add(KeySet keys, Action action);
        void Add(Keybind keybind, Action action);
        void Clear();
        bool Contains(KeySet keys);
        void Execute(KeySet keys);
    }
}
