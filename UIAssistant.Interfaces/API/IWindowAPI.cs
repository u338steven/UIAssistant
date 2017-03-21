using System;

namespace UIAssistant.Interfaces.API
{
    public interface IWindowAPI
    {
        IWindow ActiveWindow { get; }
        ITaskbar Taskbar { get; }

        void EnumerateWindows(Func<IWindow, bool> func);
        IWindow FindWindow(string className, string caption = null);
    }
}