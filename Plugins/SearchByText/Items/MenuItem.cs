﻿using System;
using System.Windows;
using UIAssistant.Interfaces.Native;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class MenuItem : SearchByTextItem
    {
        public int Id { get; set; }
        public IntPtr WindowHandle { get; set; }

        public MenuItem(string fullName, bool isEnabled, int id, IntPtr windowHandle)
            : base(fullName, fullName, Rect.Empty, isEnabled)
        {
            Id = id;
            WindowHandle = windowHandle;
        }

        const int WM_COMMAND = 0x0111;
        public override void Execute()
        {
            if (!IsEnabled)
            {
                return;
            }
            NativeMethods.PostMessage(WindowHandle, WM_COMMAND, new IntPtr(Id), IntPtr.Zero);
        }
    }
}
