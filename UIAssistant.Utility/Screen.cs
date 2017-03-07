using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.InteropServices;
using System.Windows;

using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Native;
using UIAssistant.Utility.Win32;

namespace UIAssistant.Utility
{
    public class Screen : IScreen
    {
        public Rect Bounds => new Rect(
                SystemParameters.VirtualScreenLeft,
                SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth,
                SystemParameters.VirtualScreenHeight);

        public IReadOnlyCollection<Rect> AllScreen
        {
            get
            {
                List<Rect> result = new List<Rect>();
                NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                    (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
                    {
                        var info = new MONITORINFO { cbSize = Marshal.SizeOf(typeof(MONITORINFO)) };
                        NativeMethods.GetMonitorInfo(hMonitor, info);
                        result.Add(info.rcMonitor.ToRectangle());
                        return true;
                    }, IntPtr.Zero);
                return result;
            }
        }

        public Rect GetScreenFrom(Point pt)
        {
            return AllScreen.FirstOrDefault(x => x.Contains(pt));
        }
    }

    public class CoordinateOrigin
    {
        public Rect AllScreen
        {
            get { return new Rect(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop, SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight); }
        }
        public Rect CurrentScreen
        {
            get { return new Screen().GetScreenFrom(Win32Window.ActiveWindow.Bounds.Center()).ToClientCoordinate(); }
        }
        public Rect CurrentWindow
        {
            get { return Win32Window.ActiveWindow.Bounds.ToClientCoordinate(); }
        }
    }
}
