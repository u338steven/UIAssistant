using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Threading;
using UIAssistant.Interfaces.Input;
using UIAssistant.Utility.Win32;

namespace UIAssistant.Core.Input
{
    public class MouseCursor : IMouseCursor
    {
        public bool AutoHide { get; set; }

        DispatcherTimer timer = new DispatcherTimer();
        private void RunAutoHideTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        int _timerCounter;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!AutoHide)
            {
                timer.Stop();
                SetCursorVisibility(true);
                return;
            }
            ++_timerCounter;
            if (_timerCounter > 1)
            {
                _timerCounter = 0;
                timer.Stop();
                SetCursorVisibility(false);
            }
        }

        private void AllCursorsForEach(Action<Win32Interop.OCR_SYSTEM_CURSORS> action)
        {
            Enum.GetValues(typeof(Win32Interop.OCR_SYSTEM_CURSORS)).Cast<Win32Interop.OCR_SYSTEM_CURSORS>().ForEach(x => action(x));
        }

        Dictionary<Win32Interop.OCR_SYSTEM_CURSORS, IntPtr> _blanks = new Dictionary<Win32Interop.OCR_SYSTEM_CURSORS, IntPtr>();
        Dictionary<Win32Interop.OCR_SYSTEM_CURSORS, IntPtr> _saved = new Dictionary<Win32Interop.OCR_SYSTEM_CURSORS, IntPtr>();

        public void InitializeCursor()
        {
            if (_blanks.Count > 0)
            {
                return;
            }

            var width = Win32Interop.GetSystemMetrics(Win32Interop.SystemMetric.SM_CXCURSOR);
            var height = Win32Interop.GetSystemMetrics(Win32Interop.SystemMetric.SM_CYCURSOR);
            var andMask = Enumerable.Repeat<byte>(0xff, width * height / 8).ToArray();
            var xorMask = new byte[width * height / 8];

            AllCursorsForEach(x =>
            {
                var cursor = Win32Interop.LoadCursor(IntPtr.Zero, x);
                _saved.Add(x, Win32Interop.CopyImage(cursor, Win32Interop.IMAGE_CURSOR, 0, 0, 0));
                _blanks.Add(x, Win32Interop.CreateCursor(IntPtr.Zero, 0, 0, width, height, andMask, xorMask));
            });

            _hook = new MouseHook();
            _hook.SetHook((nCode, _, __) =>
            {
                if (nCode >= 0 && !timer.IsEnabled && AutoHide)
                {
                    SetCursorVisibility(true);
                    RunAutoHideTimer();
                }
                return _hook.CallNextHookEx(nCode, _, __);
            });
        }

        MouseHook _hook;
        public void SetCursorVisibility(bool visible)
        {
            InitializeCursor();

            Dictionary<Win32Interop.OCR_SYSTEM_CURSORS, IntPtr> cursors;
            if (visible)
            {
                cursors = _saved;
            }
            else
            {
                cursors = _blanks;
            }

            cursors.ForEach(x =>
            {
                var cursor = Win32Interop.CopyImage(x.Value, Win32Interop.IMAGE_CURSOR, 0, 0, 0);
                Win32Interop.SetSystemCursor(cursor, x.Key);
            });
        }

        public void DestroyCursor()
        {
            _blanks.ForEach(x => Win32Interop.DestroyCursor(x.Value));
            _saved.Clear();
            _blanks.Clear();
            _hook?.Dispose();
        }
    }
}
