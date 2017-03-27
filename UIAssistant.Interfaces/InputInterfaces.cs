using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using KeybindHelper;
using KeybindHelper.LowLevel;
using UIAssistant.Interfaces.Native;

namespace UIAssistant.Interfaces.Input
{
    public interface IKeybindManager
    {
        Action this[KeySet keys] { get; }

        void Add(KeySet keys, Action action, bool canActWhenKeyHoldDown = false);
        void Add(Keybind keybind, Action action, bool canActWhenKeyHoldDown = false);
        bool CanActWhenKeyHoldDown(KeySet keys);
        void Clear();
        bool Contains(KeySet keys);
        void Execute(KeySet keys, bool isKeyHoldDown);
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

    public interface IKeyboardOperation
    {
        void CancelAltKey();
        void Initialize(params Key[] keys);
        void KeyDown(params Key[] keys);
        void KeyUp(params Key[] keys);
        void PressedKeyUp();
        void SendKeys(params Key[] keys);
    }

    public interface IThemeSwitchable
    {
        void SwitchNextTheme();
    }

    public interface IKeyboardPlugin : IDisposable
    {
        void Initialize(IKeyboardPluginContext context);
        void Cleanup(IKeyboardPluginContext context);
        void LoadKeybinds(IKeyboardPluginContext context);
        void OnKeyDown(IKeyboardPluginContext context, object sender, LowLevelKeyEventArgs e);
        void OnKeyUp(IKeyboardPluginContext context, object sender, LowLevelKeyEventArgs e);
    }

    public interface IKeyboardPluginContext : IDisposable
    {
        IHookHandlers HookHandlers { get; }
        IKeybindManager Keybinds { get; }
        UserControl UsagePanel { get; set; }
    }

    public interface IKeyInputController : IDisposable
    {
        void AddHidingProcess();
        void AddUsagePanelProcess(UserControl usagePanel);
        void Observe();
    }
}
