namespace UIAssistant.Interfaces.API
{
    public interface INotificationAPI
    {
        void NotifyErrorMessage(string title, string message);
        void NotifyInfoMessage(string title, string message);
        void NotifyWarnMessage(string title, string message);
    }
}