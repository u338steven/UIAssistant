using UIAssistant.Interfaces.Events;

namespace UIAssistant.Interfaces.API
{
    public interface IAutomationAPI
    {
        IEventObserver CreateObserver(ObserverKinds kind);
    }
}