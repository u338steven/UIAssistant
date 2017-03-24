using System;
using System.Windows.Automation;

namespace UIAssistant.Interfaces.Events
{
    public enum ObserverKinds
    {
        StructureChangedObserver,
        FocusObserver,
        PopupObserver,
    }

    public interface IEventObserver
    {
        Action<AutomationElement> Callback { get; set; }

        void Dispose();
        void Observe();
        bool Wait(int millisecondsTimeout = 3000);
    }
}