using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using KeybindHelper;
using KeybindHelper.LowLevel;

namespace UIAssistant.Interfaces.Input
{
    public interface IKeyboardHook : IDisposable
    {
        bool IgnoreInjected { get; set; }
        bool IsActive { get; set; }

        event LowLevelKeyEventHandler KeyDown;
        event LowLevelKeyEventHandler KeyUp;
        event LowLevelKeyEventHandler PreviewKeyDown;

        string GetKeyboardLayoutLanguage();
        void Hook();
        bool IsPressed(Key key);
        void LoadAnotherKeyboardLayout();
        void Unhook();
    }

    public interface IKeybindManager
    {
        Action this[KeySet keys] { get; }

        void Add(KeySet keys, Action action);
        void Add(Keybind keybind, Action action);
        void Clear();
        bool Contains(KeySet keys);
        void Execute(KeySet keys);
    }

    public interface IMouseCursor
    {
        bool AutoHide { get; set; }

        void DestroyCursor();
        void InitializeCursor();
        void SetCursorVisibility(bool visible);
    }

    public interface IMouseOperation
    {
        void Click(Rect bounds);
        void DoMouseEvent(params MouseEvent[] dwFlags);
        void DoMouseEvent(Rect bounds, params MouseEvent[] dwFlags);
        void DoMouseEvent(Point point, params MouseEvent[] dwFlags);
        void DoMouseEvent(double x, double y, params MouseEvent[] dwFlags);
        void DoMouseEventRelative(double x, double y, params MouseEvent[] dwFlags);
        void DoubleClick(Rect bounds);
        void DoWheelEvent(int amountOfMovement, params WheelOrientation[] orientations);
        void Drag(Rect bounds);
        void Drop(Rect bounds);
        Point GetMousePosition();
        void LeftDown();
        void LeftUp();
        void MiddleClick(Rect bounds);
        void MiddleDown();
        void MiddleUp();
        void Move(Rect bounds);
        void Move(Point pt);
        void Move(double x, double y);
        void MoveTo(Point from, Point to, int millisecondsInterval = 50, int count = 10);
        void RightClick(Rect bounds);
        void RightDown();
        void RightUp();
    }

    public enum WheelOrientation
    {
        Vertical,
        Horizontal,
    }

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

    public interface IKeyboardOperation
    {
        void CancelAltKey();
        void Initialize(params Key[] keys);
        void KeyDown(params Key[] keys);
        void KeyUp(params Key[] keys);
        void PressedKeyUp();
        void SendKeys(params Key[] keys);
    }
}
