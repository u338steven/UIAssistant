using UIAssistant.Infrastructure.Events;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Events;

namespace UIAssistant.Core.API
{
    class AutomationAPI : IAutomationAPI
    {
        public IEventObserver CreateObserver(ObserverKinds kind)
        {
            switch (kind)
            {
                case ObserverKinds.StructureChangedObserver:
                    return new StructureChangedObserver();
                case ObserverKinds.FocusObserver:
                    return new FocusObserver();
                case ObserverKinds.PopupObserver:
                    return new PopupObserver();
                default:
                    return null;
            }
        }
    }
}
