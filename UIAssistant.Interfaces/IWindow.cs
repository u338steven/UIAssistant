using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Media;

namespace UIAssistant.Interfaces
{
    [Flags]
    public enum WindowShowStyle : uint
    {
        Hide = 0,
        ShowNormal = 1,
        ShowMinimized = 2,
        ShowMaximized = 3,
        Maximize = 3,
        ShowNormalNoActivate = 4,
        Show = 5,
        Minimize = 6,
        ShowMinNoActivate = 7,
        ShowNoActivate = 8,
        Restore = 9,
        ShowDefault = 10,
        ForceMinimized = 11
    }

    public interface IWindow
    {
        Rect Bounds { get; }
        AutomationElement Element { get; }
        ImageSource Icon { get; }
        bool IsTopMost { get; }
        IWindow LastActivePopup { get; }
        string Title { get; }
        IntPtr WindowHandle { get; }

        void Activate();
        void ButtonClick();
        void Close();
        bool Equals(object obj);
        IWindow FindChild(string className, string caption = null);
        IWindow FindChild(IWindow childAfter, string className, string caption = null);
        int GetHashCode();
        bool IsAltTabWindow();
        void SetNoTopMost();
        void SetOpacity(byte value);
        void SetTopMost();
        bool SetWindowPos(IWindow insertAfterWindow, bool activate, bool hide = false);
        void ShowWindow(WindowShowStyle flags);
        void ToggleTopMost();
    }

    public interface IScreen
    {
        IReadOnlyCollection<Rect> AllScreen { get; }
        Rect Bounds { get; }

        Rect GetScreenFrom(Point pt);
    }

    public enum AppBarMessage : int
    {
        New = 0,
        Remove = 1,
        QueryPos = 2,
        SetPos = 3,
        GetState = 4,
        GetTaskBarPos = 5,
        Activate = 6,
        GetAutoHideBar = 7,
        SetAutoHideBar = 8,
        WindowPosChanged = 9,
        SetState = 10,
    }

    public enum AppBarEdge : int
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3,
    }

    public enum AppBarState : int
    {
        Autohide = 1,
        AlwaysOnTop = 2,
    }

    public interface ITaskbar
    {
        void GetBounds(out AppBarEdge edge, out Rect bounds);
        bool IsAutoHide();
    }
}
