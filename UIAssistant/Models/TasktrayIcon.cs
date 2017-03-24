using UIAssistant.Core.I18n;
using UIAssistant.Interfaces;
using UIAssistant.Utility;
using UIAssistant.Views;

namespace UIAssistant.Models
{
    class TasktrayIcon
    {
        private static NotifyIcon _notifyIcon;
        public static void ShowNotifyIcon()
        {
            _notifyIcon = new NotifyIcon(Properties.Resources.ProgramName, Properties.Resources.UIAssistant);
            _notifyIcon.AddMenuItem(TextID.TrayIconSettings.GetLocalizedText(), (_, __) => ShowSettingsWindow());
            _notifyIcon.AddMenuItem(TextID.TrayIconExit.GetLocalizedText(), (_, __) => ExitApplication());
            _notifyIcon.Show();
        }

        public static void HideNotifyIcon()
        {
            _notifyIcon?.Hide();
        }

        private static SettingsWindow _settingsWindow;
        private static void ShowSettingsWindow()
        {
            if (_settingsWindow != null && _settingsWindow.IsVisible)
            {
                _settingsWindow.Activate();
                return;
            }
            _settingsWindow = new SettingsWindow();
            _settingsWindow.Show();
        }

        private static void ExitApplication()
        {
            _settingsWindow?.Close();
            System.Windows.Application.Current?.Dispatcher.Invoke(() => System.Windows.Application.Current?.Shutdown());
        }
    }
}
