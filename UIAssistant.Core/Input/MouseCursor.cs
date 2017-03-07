using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Threading;
using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Native;

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

        private void AllCursorsForEach(Action<OCR_SYSTEM_CURSORS> action)
        {
            Enum.GetValues(typeof(OCR_SYSTEM_CURSORS)).Cast<OCR_SYSTEM_CURSORS>().ForEach(x => action(x));
        }

        Dictionary<OCR_SYSTEM_CURSORS, IntPtr> _blanks = new Dictionary<OCR_SYSTEM_CURSORS, IntPtr>();
        Dictionary<OCR_SYSTEM_CURSORS, IntPtr> _saved = new Dictionary<OCR_SYSTEM_CURSORS, IntPtr>();

        public void InitializeCursor()
        {
            if (_blanks.Count > 0)
            {
                return;
            }

            var width = NativeMethods.GetSystemMetrics(SystemMetric.SM_CXCURSOR);
            var height = NativeMethods.GetSystemMetrics(SystemMetric.SM_CYCURSOR);
            var andMask = Enumerable.Repeat<byte>(0xff, width * height / 8).ToArray();
            var xorMask = new byte[width * height / 8];

            AllCursorsForEach(x =>
            {
                var cursor = NativeMethods.LoadCursor(IntPtr.Zero, x);
                _saved.Add(x, NativeMethods.CopyImage(cursor, NativeMethods.IMAGE_CURSOR, 0, 0, 0));
                _blanks.Add(x, NativeMethods.CreateCursor(IntPtr.Zero, 0, 0, width, height, andMask, xorMask));
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

            Dictionary<OCR_SYSTEM_CURSORS, IntPtr> cursors;
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
                var cursor = NativeMethods.CopyImage(x.Value, NativeMethods.IMAGE_CURSOR, 0, 0, 0);
                NativeMethods.SetSystemCursor(cursor, x.Key);
            });
        }

        public void DestroyCursor()
        {
            _blanks.ForEach(x => NativeMethods.DestroyCursor(x.Value));
            _saved.Clear();
            _blanks.Clear();
            _hook?.Dispose();
        }
    }
}
