using System;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.API;
using UIAssistant.Utility.Win32;

namespace UIAssistant.Core.API
{
    class WindowAPI : IWindowAPI
    {
        public IWindow ActiveWindow { get { return Win32Window.ActiveWindow; } }
        public ITaskbar Taskbar { get { return new Win32Taskbar(); } }
        public IWindow FindWindow(string className, string caption = null)
        {
            return Win32Window.Find(className, caption);
        }

        public void EnumerateWindows(Func<IWindow, bool> func)
        {
            Win32Window.Filter(func);
        }
    }
}
