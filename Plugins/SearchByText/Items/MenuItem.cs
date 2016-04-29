using System;
using System.Windows;
using UIAssistant.Utility.Win32;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class MenuItem : SearchByTextItem
    {
        public int Id { get; set; }

        public MenuItem(string fullName, bool isEnabled, int id)
            : base(fullName, fullName, Rect.Empty, isEnabled)
        {
            Id = id;
        }

        const int WM_COMMAND = 0x0111;
        public override void Execute()
        {
            if (!IsEnabled)
            {
                return;
            }
            Win32Interop.PostMessage(Win32Window.ActiveWindow.WindowHandle, WM_COMMAND, new IntPtr(Id), IntPtr.Zero);
        }
    }
}
