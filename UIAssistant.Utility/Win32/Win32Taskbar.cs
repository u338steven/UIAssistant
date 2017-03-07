using System;
using System.Runtime.InteropServices;

using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Native;

namespace UIAssistant.Utility.Win32
{
    public class Win32Taskbar : ITaskbar
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            [MarshalAs(UnmanagedType.U4)]
            public AppBarEdge uEdge;
            public RECT rc;
            public int lParam;
        }

        [DllImport("shell32.dll")]
        private static extern int SHAppBarMessage(AppBarMessage dwMessage, ref APPBARDATA pData);

        public bool IsAutoHide()
        {
            var barData = new APPBARDATA();

            if ((SHAppBarMessage(AppBarMessage.GetState, ref barData) & (int)AppBarState.Autohide) > 0)
            {
                return true;
            }
            return false;
        }

        public void GetBounds(out AppBarEdge edge, out System.Windows.Rect bounds)
        {
            var barData = new APPBARDATA();
            int ret = SHAppBarMessage(AppBarMessage.GetTaskBarPos, ref barData);

            edge = barData.uEdge;
            bounds = barData.rc.ToRectangle();
        }
    }
}
