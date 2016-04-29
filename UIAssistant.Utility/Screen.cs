using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Windows;

using UIAssistant.Utility.Win32;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Utility
{
    public static class Screen
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public Win32Interop.Rect rcMonitor = new Win32Interop.Rect();
            public Win32Interop.Rect rcWork = new Win32Interop.Rect();
            public int dwFlags = 0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice = new char[32];
        }

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);
        public delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Win32Interop.Rect lprcMonitor, IntPtr dwData);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hmonitor, [In, Out]MONITORINFO info);

        public static Rect Bounds => new Rect(
                SystemParameters.VirtualScreenLeft,
                SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth,
                SystemParameters.VirtualScreenHeight);

        public static List<Rect> AllScreen => GetAllScreen();
        private static List<Rect> GetAllScreen()
        {
            List<Rect> result = new List<Rect>();
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                (IntPtr hMonitor, IntPtr hdcMonitor, ref Win32Interop.Rect lprcMonitor, IntPtr dwData) =>
                {
                    var info = new MONITORINFO { cbSize = Marshal.SizeOf(typeof(MONITORINFO)) };
                    GetMonitorInfo(hMonitor, info);
                    result.Add(info.rcMonitor.ToRectangle());
                    return true;
                }, IntPtr.Zero);
            return result;
        }

        public static Rect GetScreenFrom(Point pt)
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
            get { return Screen.GetScreenFrom(Win32Window.ActiveWindow.Bounds.Center()).ToClientCoordinate(); }
        }
        public Rect CurrentWindow
        {
            get { return Win32Window.ActiveWindow.Bounds.ToClientCoordinate(); }
        }
    }
}
