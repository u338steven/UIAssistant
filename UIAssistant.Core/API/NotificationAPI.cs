using UIAssistant.Interfaces.API;
using UIAssistant.UI.Controls;

namespace UIAssistant.Core.API
{
    class NotificationAPI : INotificationAPI
    {
        public void NotifyWarnMessage(string title, string message)
        {
            Notification.NotifyMessage(title, message, NotificationIcon.Warning);
        }

        public void NotifyInfoMessage(string title, string message)
        {
            Notification.NotifyMessage(title, message, NotificationIcon.Information);
        }

        public void NotifyErrorMessage(string title, string message)
        {
            Notification.NotifyMessage(title, message, NotificationIcon.Error);
        }
    }
}
