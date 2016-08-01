using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace UIAssistant.Utility.Win32
{
    public static class Win32Interop
    {
        #region HRESULT
        public enum HRESULT : long
        {
            S_FALSE = 0x0001,
            S_OK = 0x0000,
            E_INVALIDARG = 0x80070057,
            E_OUTOFMEMORY = 0x8007000E
        }

        public static bool HResultHasError(int hResult)
        {
            return Convert.ToInt32(HRESULT.S_OK) != hResult;
        }
        #endregion

        #region Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public Rect(System.Windows.Rect rect)
            {
                left = (int)rect.Left;
                top = (int)rect.Top;
                right = (int)rect.Right;
                bottom = (int)rect.Bottom;
            }

            public System.Windows.Rect ToRectangle()
            {
                return new System.Windows.Rect(left, top, right - left, bottom - top);
            }

            public int width
            {
                get { return right - left; }
            }

            public int height
            {
                get { return bottom - top; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;

            public Point(int dx, int dy)
            {
                x = dx;
                y = dy;
            }
        }

        #endregion

        #region AttachThreadInput
        public const int SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000;
        public const int SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001;

        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static void AttachedThreadInputAction(Action action)
        {
            var foreHandle = GetForegroundWindow();
            if (foreHandle == IntPtr.Zero)
            {
                action();
                return;
            }
            var foreThread = GetWindowThreadProcessId(foreHandle, IntPtr.Zero);
            var appThread = GetCurrentThreadId();
            bool threadsAttached = false;

            try
            {
                threadsAttached =
                    foreThread == appThread || AttachThreadInput(appThread, foreThread, true);
                action();
            }
            finally
            {
                if (threadsAttached)
                    AttachThreadInput(appThread, foreThread, false);
            }
        }
        #endregion

        #region Get/SetWindowLong/Ptr
        public enum GWL : int
        {
            GWL_WNDPROC = (-4),
            GWL_HINSTANCE = (-6),
            GWL_HWNDPARENT = (-8),
            GWL_STYLE = (-16),
            GWL_EXSTYLE = (-20),
            GWL_USERDATA = (-21),
            GWL_ID = (-12)
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);

        public static IntPtr GetWindowLongPtr(IntPtr hWnd, GWL nIndex)
        {
            if (Environment.Is64BitProcess)
                return GetWindowLongPtr64(hWnd, nIndex);
            else
                return GetWindowLongPtr32(hWnd, nIndex);
        }

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong)
        {
            if (Environment.Is64BitProcess)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        const int WS_EX_TOOLWINDOW = 0x00000080;
        const int WS_EX_TRANSPARENT = 0x00000020;

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLongPtr(hwnd, GWL.GWL_EXSTYLE);
            SetWindowLongPtr(hwnd, GWL.GWL_EXSTYLE, new IntPtr(extendedStyle.ToInt32() | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW));
        }
        #endregion

        #region Send/PostMessage
        public const int BM_CLICK = 0x00F5;
        public const int TCM_FIRST = 0x1300;
        public const int TCM_GETITEMCOUNT = (TCM_FIRST + 4);
        public const int TCM_GETITEMRECT = (TCM_FIRST + 10);
        public const int TCM_GETCURSEL = (TCM_FIRST + 11);
        public const int TCM_SETCURSEL = (TCM_FIRST + 12);
        public const int TCM_GETROWCOUNT = (TCM_FIRST + 44);
        public const int TCM_GETTOOLTIPS = (TCM_FIRST + 45);
        public const int TCM_SETTOOLTIPS = (TCM_FIRST + 46);
        public const int TCM_GETCURFOCUS = (TCM_FIRST + 47);
        public const int TCM_SETCURFOCUS = (TCM_FIRST + 48);
        public const int TCM_DESELECTALL = (TCM_FIRST + 50);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        #endregion

        #region SendInput
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_MOUSE = 0;

        public enum KeyEvent : int
        {
            KEYEVENTF_KEYDOWN = 0x0,
            KEYEVENTF_EXTENDEDKEY = 0x1,
            KEYEVENTF_KEYUP = 0x2,
            KEYEVENTF_UNICODE = 0x4,
            KEYEVENTF_SCANCODE = 0x8,
        }

        public enum MouseEvent : int
        {
            MOUSEEVENTF_MOVED = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_HWHEEL = 0x1000,
            MOUSEEVENTF_ABSOLUTE = 0x8000,
            MOUSEEVENTF_XBUTTON1 = 0x0001,
            MOUSEEVENTF_XBUTTON2 = 0x0002,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;
            public INPUTUNION iu;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public MouseEvent dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public KeyEvent dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Win32Interop.Point pt);
        #endregion
    }
}
