using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using KongariToast;

namespace UIAssistant.UI.Controls
{
    public enum NotificationIcon
    {
        None,
        Information,
        Warning,
        Question,
        Error,
    }

    public static class Notification
    {
        private static ToastNotifications _toast;

        public static void Initialize()
        {
            _toast = new ToastNotifications();
        }

        public static void NotifyMessage(string title, string message, NotificationIcon icon = NotificationIcon.None, ImageSource image = null)
        {
            NotificationType type;
            switch (icon)
            {
                case NotificationIcon.None:
                    type = NotificationType.None;
                    break;
                case NotificationIcon.Information:
                    type = NotificationType.Information;
                    break;
                case NotificationIcon.Warning:
                    type = NotificationType.Warning;
                    break;
                case NotificationIcon.Question:
                    type = NotificationType.Question;
                    break;
                case NotificationIcon.Error:
                    type = NotificationType.Error;
                    break;
                default:
                    type = NotificationType.None;
                    break;
            }
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                _toast.AddNotification(new KongariToast.Notification { Title = title, Message = message, Image = image, NotificationType = type })));
        }
    }
}
