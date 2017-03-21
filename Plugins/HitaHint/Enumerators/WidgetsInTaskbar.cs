using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Automation;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.HitaHint.Enumerators
{
    class WidgetsInTaskbar : IWidgetEnumerator
    {
        public const string TaskbarClass = "Shell_TrayWnd";
        public const string TasktrayClass = "TrayNotifyWnd";
        public const string NotifyIconOverflowClass = "NotifyIconOverflowWindow";

        public void Enumerate(ICollection<IHUDItem> container)
        {
            var targetWindow = HitaHint.UIAssistantAPI.WindowAPI.FindWindow(TaskbarClass, "");
            targetWindow.Activate();
            var enumerator = new WidgetEnumerator();
            enumerator.Enumerate(container, null, _enumerateTargets);
            var taskbar = HitaHint.UIAssistantAPI.WindowAPI.Taskbar;

            double x = 0, y = 0;
            if (taskbar.IsAutoHide())
            {
                var adjuster = new System.Windows.Point();
                AppBarEdge edge;
                System.Windows.Rect bounds;
                taskbar.GetBounds(out edge, out bounds);
                bounds = bounds.ToClientCoordinate();

                double space = 8;
                switch (edge)
                {
                    case AppBarEdge.Top:
                        x = bounds.Left + space;
                        y = bounds.Top;
                        break;
                    case AppBarEdge.Bottom:
                        x = bounds.Left + space;
                        y = bounds.Bottom;
                        break;
                    case AppBarEdge.Left:
                        x = bounds.Left;
                        y = bounds.Top + space;
                        break;
                    case AppBarEdge.Right:
                        x = bounds.Right;
                        y = bounds.Top + space;
                        break;
                }

                var rect = targetWindow.Bounds.ToClientCoordinate();

                adjuster.X = bounds.Left - rect.Left;
                adjuster.Y = bounds.Top - rect.Top;
                container.ToList().ForEach(item => item.Bounds.Offset(adjuster.X, adjuster.Y));
                HitaHint.UIAssistantAPI.MouseAPI.MouseOperation.DoMouseEvent(x, y);
            }

            if (TryShowNotifyIconOverflow(targetWindow))
            {
                _isVisibleNotifyIconOverflow = true;
                enumerator.Retry(container);
            }
        }

        public void Dispose()
        {
            Task.Run(() =>
            {
                System.Threading.Thread.Sleep(300);
                if (!_isVisibleNotifyIconOverflow)
                {
                    return;
                }
                var overflowWindow = HitaHint.UIAssistantAPI.WindowAPI.FindWindow(NotifyIconOverflowClass, "");
                overflowWindow.ShowWindow(WindowShowStyle.Hide);
            });
        }

        private bool _isVisibleNotifyIconOverflow = false;
        private static bool TryShowNotifyIconOverflow(IWindow taskbar)
        {
            var overflowButton = taskbar.FindChild(TasktrayClass, "").FindChild("Button", "");
            if (overflowButton == null)
            {
                return false;
            }
            overflowButton.ButtonClick();
            overflowButton.ButtonClick();
            System.Threading.Thread.Sleep(100);
            var overflowWindow = HitaHint.UIAssistantAPI.WindowAPI.FindWindow(NotifyIconOverflowClass, "");
            overflowWindow.ShowWindow(WindowShowStyle.Show);
            return true;
        }

        #region Targets for Taskbar
        private static readonly ControlType[] _enumerateTargets =
        {
            ControlType.Button,
            ControlType.Calendar,
            ControlType.CheckBox,
            ControlType.ComboBox,
            ControlType.Custom,
            ControlType.DataGrid,
            ControlType.DataItem,
            ControlType.Edit,
            ControlType.HeaderItem,
            ControlType.Hyperlink,
            ControlType.Image,
            ControlType.ListItem,
            ControlType.MenuItem,
            ControlType.ProgressBar,
            ControlType.RadioButton,
            ControlType.Slider,
            ControlType.Spinner,
            ControlType.SplitButton,
            ControlType.TabItem,
            ControlType.Table,
            ControlType.Thumb,
            ControlType.TreeItem,
        };
        #endregion
    }
}
