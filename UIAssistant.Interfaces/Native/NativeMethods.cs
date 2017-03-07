using System;
using System.Runtime.InteropServices;

namespace UIAssistant.Interfaces.Native
{
    public enum InputKind : int
    {
        INPUT_MOUSE = 0,
        INPUT_KEYBOARD = 1,
    }

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
        public InputKind type;
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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public RECT rcMonitor = new RECT();
        public RECT rcWork = new RECT();
        public int dwFlags = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] szDevice = new char[32];
    }

    public enum OCR_SYSTEM_CURSORS : uint
    {
        /// <summary>
        /// Standard arrow and small hourglass
        /// </summary>
        OCR_APPSTARTING = 32650,
        /// <summary>
        /// Standard arrow
        /// </summary>
        OCR_NORMAL = 32512,
        /// <summary>
        /// Crosshair
        /// </summary>
        OCR_CROSS = 32515,
        /// <summary>
        /// Windows 2000/XP: Hand
        /// </summary>
        OCR_HAND = 32649,
        /// <summary>
        /// Arrow and question mark
        /// </summary>
        OCR_HELP = 32651,
        /// <summary>
        /// I-beam
        /// </summary>
        OCR_IBEAM = 32513,
        /// <summary>
        /// Slashed circle
        /// </summary>
        OCR_NO = 32648,
        /// <summary>
        /// Four-pointed arrow pointing north, south, east, and west
        /// </summary>
        OCR_SIZEALL = 32646,
        /// <summary>
        /// Double-pointed arrow pointing northeast and southwest
        /// </summary>
        OCR_SIZENESW = 32643,
        /// <summary>
        /// Double-pointed arrow pointing north and south
        /// </summary>
        OCR_SIZENS = 32645,
        /// <summary>
        /// Double-pointed arrow pointing northwest and southeast
        /// </summary>
        OCR_SIZENWSE = 32642,
        /// <summary>
        /// Double-pointed arrow pointing west and east
        /// </summary>
        OCR_SIZEWE = 32644,
        /// <summary>
        /// Vertical arrow
        /// </summary>
        OCR_UP = 32516,
        /// <summary>
        /// Hourglass
        /// </summary>
        OCR_WAIT = 32514
    }

    public enum HRESULT : long
    {
        S_FALSE = 0x0001,
        S_OK = 0x0000,
        E_INVALIDARG = 0x80070057,
        E_OUTOFMEMORY = 0x8007000E
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public RECT(System.Windows.Rect rect)
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
    public struct POINT
    {
        public int x;
        public int y;

        public POINT(int dx, int dy)
        {
            x = dx;
            y = dy;
        }
    }

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

    public enum SystemMetric
    {
        SM_CXSCREEN = 0,  // 0x00
        SM_CYSCREEN = 1,  // 0x01
        SM_CXVSCROLL = 2,  // 0x02
        SM_CYHSCROLL = 3,  // 0x03
        SM_CYCAPTION = 4,  // 0x04
        SM_CXBORDER = 5,  // 0x05
        SM_CYBORDER = 6,  // 0x06
        SM_CXDLGFRAME = 7,  // 0x07
        SM_CXFIXEDFRAME = 7,  // 0x07
        SM_CYDLGFRAME = 8,  // 0x08
        SM_CYFIXEDFRAME = 8,  // 0x08
        SM_CYVTHUMB = 9,  // 0x09
        SM_CXHTHUMB = 10, // 0x0A
        SM_CXICON = 11, // 0x0B
        SM_CYICON = 12, // 0x0C
        SM_CXCURSOR = 13, // 0x0D
        SM_CYCURSOR = 14, // 0x0E
        SM_CYMENU = 15, // 0x0F
        SM_CXFULLSCREEN = 16, // 0x10
        SM_CYFULLSCREEN = 17, // 0x11
        SM_CYKANJIWINDOW = 18, // 0x12
        SM_MOUSEPRESENT = 19, // 0x13
        SM_CYVSCROLL = 20, // 0x14
        SM_CXHSCROLL = 21, // 0x15
        SM_DEBUG = 22, // 0x16
        SM_SWAPBUTTON = 23, // 0x17
        SM_CXMIN = 28, // 0x1C
        SM_CYMIN = 29, // 0x1D
        SM_CXSIZE = 30, // 0x1E
        SM_CYSIZE = 31, // 0x1F
        SM_CXSIZEFRAME = 32, // 0x20
        SM_CXFRAME = 32, // 0x20
        SM_CYSIZEFRAME = 33, // 0x21
        SM_CYFRAME = 33, // 0x21
        SM_CXMINTRACK = 34, // 0x22
        SM_CYMINTRACK = 35, // 0x23
        SM_CXDOUBLECLK = 36, // 0x24
        SM_CYDOUBLECLK = 37, // 0x25
        SM_CXICONSPACING = 38, // 0x26
        SM_CYICONSPACING = 39, // 0x27
        SM_MENUDROPALIGNMENT = 40, // 0x28
        SM_PENWINDOWS = 41, // 0x29
        SM_DBCSENABLED = 42, // 0x2A
        SM_CMOUSEBUTTONS = 43, // 0x2B
        SM_SECURE = 44, // 0x2C
        SM_CXEDGE = 45, // 0x2D
        SM_CYEDGE = 46, // 0x2E
        SM_CXMINSPACING = 47, // 0x2F
        SM_CYMINSPACING = 48, // 0x30
        SM_CXSMICON = 49, // 0x31
        SM_CYSMICON = 50, // 0x32
        SM_CYSMCAPTION = 51, // 0x33
        SM_CXSMSIZE = 52, // 0x34
        SM_CYSMSIZE = 53, // 0x35
        SM_CXMENUSIZE = 54, // 0x36
        SM_CYMENUSIZE = 55, // 0x37
        SM_ARRANGE = 56, // 0x38
        SM_CXMINIMIZED = 57, // 0x39
        SM_CYMINIMIZED = 58, // 0x3A
        SM_CXMAXTRACK = 59, // 0x3B
        SM_CYMAXTRACK = 60, // 0x3C
        SM_CXMAXIMIZED = 61, // 0x3D
        SM_CYMAXIMIZED = 62, // 0x3E
        SM_NETWORK = 63, // 0x3F
        SM_CLEANBOOT = 67, // 0x43
        SM_CXDRAG = 68, // 0x44
        SM_CYDRAG = 69, // 0x45
        SM_SHOWSOUNDS = 70, // 0x46
        SM_CXMENUCHECK = 71, // 0x47
        SM_CYMENUCHECK = 72, // 0x48
        SM_SLOWMACHINE = 73, // 0x49
        SM_MIDEASTENABLED = 74, // 0x4A
        SM_MOUSEWHEELPRESENT = 75, // 0x4B
        SM_XVIRTUALSCREEN = 76, // 0x4C
        SM_YVIRTUALSCREEN = 77, // 0x4D
        SM_CXVIRTUALSCREEN = 78, // 0x4E
        SM_CYVIRTUALSCREEN = 79, // 0x4F
        SM_CMONITORS = 80, // 0x50
        SM_SAMEDISPLAYFORMAT = 81, // 0x51
        SM_IMMENABLED = 82, // 0x52
        SM_CXFOCUSBORDER = 83, // 0x53
        SM_CYFOCUSBORDER = 84, // 0x54
        SM_TABLETPC = 86, // 0x56
        SM_MEDIACENTER = 87, // 0x57
        SM_STARTER = 88, // 0x58
        SM_SERVERR2 = 89, // 0x59
        SM_MOUSEHORIZONTALWHEELPRESENT = 91, // 0x5B
        SM_CXPADDEDBORDER = 92, // 0x5C
        SM_DIGITIZER = 94, // 0x5E
        SM_MAXIMUMTOUCHES = 95, // 0x5F

        SM_REMOTESESSION = 0x1000, // 0x1000
        SM_SHUTTINGDOWN = 0x2000, // 0x2000
        SM_REMOTECONTROL = 0x2001, // 0x2001


        SM_CONVERTABLESLATEMODE = 0x2003,
        SM_SYSTEMDOCKED = 0x2004,
    }

    public class NativeMethods
    {
        #region HRESULT
        public static bool HResultHasError(int hResult)
        {
            return Convert.ToInt32(HRESULT.S_OK) != hResult;
        }
        #endregion

        #region SystemInfo
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);
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

        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        public static void SetWindowTransparent(IntPtr hwnd, byte alpha)
        {
            SetWindowExTransparent(hwnd);
            SetLayeredWindowAttributes(hwnd, 0, alpha, LWA_ALPHA);
        }
        #endregion

        #region Send/PostMessage
        public const int WM_CLOSE = 0x0010;
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

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        #endregion

        #region Cursor
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref POINT pt);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateCursor(IntPtr hInst, int xHotSpot, int yHotSpot,
          int nWidth, int nHeight, byte[] pvANDPlane, byte[] pvXORPlane);

        [DllImport("user32.dll")]
        public static extern bool DestroyCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, OCR_SYSTEM_CURSORS lpCursorName);

        /// <summary>
        /// Loads a bitmap.
        /// </summary>
        public const int IMAGE_BITMAP = 0;

        /// <summary>
        /// Loads an icon.
        /// </summary>
        public const int IMAGE_ICON = 1;

        /// <summary>
        /// Loads a cursor.
        /// </summary>
        public const int IMAGE_CURSOR = 2;

        /// <summary>
        /// Loads an enhanced metafile.
        /// </summary>
        public const int IMAGE_ENHMETAFILE = 3;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CopyImage(IntPtr hImage, uint uType, int cxDesired, int cyDesired, uint fuFlags);

        [DllImport("user32.dll")]
        public static extern bool SetSystemCursor(IntPtr hcur, OCR_SYSTEM_CURSORS id);
        #endregion

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);
        public delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hmonitor, [In, Out]MONITORINFO info);
    }
}
