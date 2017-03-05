using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Runtime.InteropServices;

namespace UIAssistant.Infrastructure.Events
{
    public abstract class EventHook : IDisposable
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
           IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        #region constants
        public const uint WINEVENT_OUTOFCONTEXT = 0x0000; // Events are ASYNC
        public const uint WINEVENT_SKIPOWNTHREAD = 0x0001; // Don't call back for events on installer's thread
        public const uint WINEVENT_SKIPOWNPROCESS = 0x0002; // Don't call back for events on installer's process
        public const uint WINEVENT_INCONTEXT = 0x0004; // Events are SYNC, this causes your dll to be injected into every process
        public const uint EVENT_MIN = 0x00000001;
        public const uint EVENT_MAX = 0x7FFFFFFF;
        public const uint EVENT_SYSTEM_SOUND = 0x0001;
        public const uint EVENT_SYSTEM_ALERT = 0x0002;
        public const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        public const uint EVENT_SYSTEM_MENUSTART = 0x0004;
        public const uint EVENT_SYSTEM_MENUEND = 0x0005;
        public const uint EVENT_SYSTEM_MENUPOPUPSTART = 0x0006;
        public const uint EVENT_SYSTEM_MENUPOPUPEND = 0x0007;
        public const uint EVENT_SYSTEM_CAPTURESTART = 0x0008;
        public const uint EVENT_SYSTEM_CAPTUREEND = 0x0009;
        public const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
        public const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;
        public const uint EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C;
        public const uint EVENT_SYSTEM_CONTEXTHELPEND = 0x000D;
        public const uint EVENT_SYSTEM_DRAGDROPSTART = 0x000E;
        public const uint EVENT_SYSTEM_DRAGDROPEND = 0x000F;
        public const uint EVENT_SYSTEM_DIALOGSTART = 0x0010;
        public const uint EVENT_SYSTEM_DIALOGEND = 0x0011;
        public const uint EVENT_SYSTEM_SCROLLINGSTART = 0x0012;
        public const uint EVENT_SYSTEM_SCROLLINGEND = 0x0013;
        public const uint EVENT_SYSTEM_SWITCHSTART = 0x0014;
        public const uint EVENT_SYSTEM_SWITCHEND = 0x0015;
        public const uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;
        public const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;
        public const uint EVENT_SYSTEM_DESKTOPSWITCH = 0x0020;
        public const uint EVENT_SYSTEM_END = 0x00FF;
        public const uint EVENT_OEM_DEFINED_START = 0x0101;
        public const uint EVENT_OEM_DEFINED_END = 0x01FF;
        public const uint EVENT_UIA_EVENTID_START = 0x4E00;
        public const uint EVENT_UIA_EVENTID_END = 0x4EFF;
        public const uint EVENT_UIA_PROPID_START = 0x7500;
        public const uint EVENT_UIA_PROPID_END = 0x75FF;
        public const uint EVENT_CONSOLE_CARET = 0x4001;
        public const uint EVENT_CONSOLE_UPDATE_REGION = 0x4002;
        public const uint EVENT_CONSOLE_UPDATE_SIMPLE = 0x4003;
        public const uint EVENT_CONSOLE_UPDATE_SCROLL = 0x4004;
        public const uint EVENT_CONSOLE_LAYOUT = 0x4005;
        public const uint EVENT_CONSOLE_START_APPLICATION = 0x4006;
        public const uint EVENT_CONSOLE_END_APPLICATION = 0x4007;
        public const uint EVENT_CONSOLE_END = 0x40FF;
        public const uint EVENT_OBJECT_CREATE = 0x8000; // hwnd ID idChild is created item
        public const uint EVENT_OBJECT_DESTROY = 0x8001; // hwnd ID idChild is destroyed item
        public const uint EVENT_OBJECT_SHOW = 0x8002; // hwnd ID idChild is shown item
        public const uint EVENT_OBJECT_HIDE = 0x8003; // hwnd ID idChild is hidden item
        public const uint EVENT_OBJECT_REORDER = 0x8004; // hwnd ID idChild is parent of zordering children
        public const uint EVENT_OBJECT_FOCUS = 0x8005; // hwnd ID idChild is focused item
        public const uint EVENT_OBJECT_SELECTION = 0x8006; // hwnd ID idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
        public const uint EVENT_OBJECT_SELECTIONADD = 0x8007; // hwnd ID idChild is item added
        public const uint EVENT_OBJECT_SELECTIONREMOVE = 0x8008; // hwnd ID idChild is item removed
        public const uint EVENT_OBJECT_SELECTIONWITHIN = 0x8009; // hwnd ID idChild is parent of changed selected items
        public const uint EVENT_OBJECT_STATECHANGE = 0x800A; // hwnd ID idChild is item w/ state change
        public const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B; // hwnd ID idChild is moved/sized item
        public const uint EVENT_OBJECT_NAMECHANGE = 0x800C; // hwnd ID idChild is item w/ name change
        public const uint EVENT_OBJECT_DESCRIPTIONCHANGE = 0x800D; // hwnd ID idChild is item w/ desc change
        public const uint EVENT_OBJECT_VALUECHANGE = 0x800E; // hwnd ID idChild is item w/ value change
        public const uint EVENT_OBJECT_PARENTCHANGE = 0x800F; // hwnd ID idChild is item w/ new parent
        public const uint EVENT_OBJECT_HELPCHANGE = 0x8010; // hwnd ID idChild is item w/ help change
        public const uint EVENT_OBJECT_DEFACTIONCHANGE = 0x8011; // hwnd ID idChild is item w/ def action change
        public const uint EVENT_OBJECT_ACCELERATORCHANGE = 0x8012; // hwnd ID idChild is item w/ keybd accel change
        public const uint EVENT_OBJECT_INVOKED = 0x8013; // hwnd ID idChild is item invoked
        public const uint EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x8014; // hwnd ID idChild is item w? test selection change
        public const uint EVENT_OBJECT_CONTENTSCROLLED = 0x8015;
        public const uint EVENT_SYSTEM_ARRANGMENTPREVIEW = 0x8016;
        public const uint EVENT_OBJECT_END = 0x80FF;
        public const uint EVENT_AIA_START = 0xA000;
        public const uint EVENT_AIA_END = 0xAFFF;
        #endregion

        IntPtr _hook = IntPtr.Zero;
        public bool OnlyOnceCallback { get; set; } = false;

        public EventHook()
        {

        }

        protected virtual void Hook(uint eventMin, uint eventMax)
        {
            _hook = SetWinEventHook(eventMin, eventMax, IntPtr.Zero, WindowEventCallback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
        }

        public void Unhook()
        {
            if (_hook != IntPtr.Zero)
            {
                UnhookWinEvent(_hook);
                _hook = IntPtr.Zero;
            }
        }

        public event Action<IntPtr> Callback;
        private void WindowEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Callback?.Invoke(hwnd);
            autoEvents[0].Set();
            if (OnlyOnceCallback)
            {
                Unhook();
                Callback = null;
            }
        }

        private AutoResetEvent[] autoEvents;
        public bool WaitCallback(int timeout)
        {
            autoEvents = new AutoResetEvent[]
            {
                new AutoResetEvent(false)
            };

            return WaitHandle.WaitAll(autoEvents, timeout, true);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    Callback = null;
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。
                Unhook();
                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~EventHook()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class PopupHook : EventHook
    {
        public void Observe()
        {
            Hook(EVENT_SYSTEM_MENUPOPUPSTART, EVENT_SYSTEM_MENUPOPUPSTART);
        }
    }

    public class WindowHook : EventHook
    {
        public void Observe()
        {
            Hook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND);
        }
    }

    public class FocusHook : EventHook
    {
        public void Observe()
        {
            Hook(EVENT_OBJECT_FOCUS, EVENT_OBJECT_FOCUS);
        }
    }
}
