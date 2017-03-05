using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Automation;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Utility.Win32;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.HitaHint.Enumerators
{
    class WidgetsInTaskbar : IWidgetEnumerator
    {
        public const string TaskbarClass = "Shell_TrayWnd";
        public const string TasktrayClass = "TrayNotifyWnd";
        public const string NotifyIconOverflowClass = "NotifyIconOverflowWindow";

        public void Enumerate(ICollection<IHUDItem> container)
        {
            var targetWindow = Win32Window.Find(TaskbarClass, "");
            targetWindow.Activate();
            var enumerator = new WidgetEnumerator();
            enumerator.Enumerate(container, null, _enumerateTargets);

            double x = 0, y = 0;
            if (Win32Taskbar.IsAutoHide())
            {
                var adjuster = new System.Windows.Point();
                Win32Taskbar.AppBarEdge edge;
                System.Windows.Rect bounds;
                Win32Taskbar.GetBounds(out edge, out bounds);
                bounds = bounds.ToClientCoordinate();

                double space = 8;
                switch (edge)
                {
                    case Win32Taskbar.AppBarEdge.Top:
                        x = bounds.Left + space;
                        y = bounds.Top;
                        break;
                    case Win32Taskbar.AppBarEdge.Bottom:
                        x = bounds.Left + space;
                        y = bounds.Bottom;
                        break;
                    case Win32Taskbar.AppBarEdge.Left:
                        x = bounds.Left;
                        y = bounds.Top + space;
                        break;
                    case Win32Taskbar.AppBarEdge.Right:
                        x = bounds.Right;
                        y = bounds.Top + space;
                        break;
                }

                var rect = targetWindow.Bounds.ToClientCoordinate();

                adjuster.X = bounds.Left - rect.Left;
                adjuster.Y = bounds.Top - rect.Top;
                container.ForEach(item => item.Bounds.Offset(adjuster.X, adjuster.Y));
                HitaHint.UIAssistantAPI.MouseOperation.DoMouseEvent(x, y);
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
                var overflowWindow = Win32Window.Find(NotifyIconOverflowClass, "");
                overflowWindow.ShowWindow(WindowShowStyle.Hide);
            });
        }

        private bool _isVisibleNotifyIconOverflow = false;
        private static bool TryShowNotifyIconOverflow(Win32Window taskbar)
        {
            var overflowButton = taskbar.FindChild(TasktrayClass, "").FindChild("Button", "");
            if (overflowButton == null)
            {
                return false;
            }
            overflowButton.ButtonClick();
            overflowButton.ButtonClick();
            System.Threading.Thread.Sleep(100);
            var overflowWindow = Win32Window.Find(NotifyIconOverflowClass, "");
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
