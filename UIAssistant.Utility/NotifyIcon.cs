using System;
using System.Collections.Generic;

using System.Windows.Forms;
using Forms = System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace UIAssistant.Utility
{
    public class NotifyIcon : IDisposable
    {
        private Forms.NotifyIcon _notifyIcon;
        private List<MenuItem> _menuItems = new List<MenuItem>();

        public NotifyIcon(string name, Icon icon)
        {
            _notifyIcon = new Forms.NotifyIcon
            {
                Text = name,
                Icon = icon,
            };
        }

        public void AddMenuItem(string name, EventHandler action)
        {
            var item = new MenuItem(name);
            item.Click += action;
            _menuItems.Add(item);
        }

        const int MAX_RETRY = 5;
        const int REGISTER_TIMEOUT = 4000;
        public void Show()
        {
            _notifyIcon.ContextMenu = new ContextMenu(_menuItems.ToArray());

            var stopwatch = new Stopwatch();
            for (var i = 0; i < MAX_RETRY; ++i)
            {
                stopwatch.Start();
                _notifyIcon.Visible = true;
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds < REGISTER_TIMEOUT)
                {
                    return;
                }
                _notifyIcon.Visible = false;
                stopwatch.Reset();
            }
        }

        public void Hide()
        {
            _notifyIcon.Visible = false;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Hide();
                    _notifyIcon?.Dispose();
                }
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
