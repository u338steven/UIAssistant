using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace UIAssistant.Utility.Win32
{
    public class Win32Taskbar
    {
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

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            [MarshalAs(UnmanagedType.U4)]
            public AppBarEdge uEdge;
            public Win32Interop.Rect rc;
            public int lParam;
        }

        [DllImport("shell32.dll")]
        private static extern int SHAppBarMessage(AppBarMessage dwMessage, ref APPBARDATA pData);

        public static bool IsAutoHide()
        {
            var barData = new APPBARDATA();

            if ((SHAppBarMessage(AppBarMessage.GetState, ref barData) & (int)AppBarState.Autohide) > 0)
            {
                return true;
            }
            return false;
        }

        public static void GetBounds(out AppBarEdge edge, out System.Windows.Rect bounds)
        {
            var barData = new APPBARDATA();
            int ret = SHAppBarMessage(AppBarMessage.GetTaskBarPos, ref barData);

            edge = barData.uEdge;
            bounds = barData.rc.ToRectangle();
        }
    }
}
