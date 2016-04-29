using System;
using System.Windows;
using System.Windows.Automation;
using UIAssistant.Utility.Win32;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class TextItemWithTab : SearchByTextItem
    {
        AutomationElement TabItem { get; set; }
        IntPtr TabHandle { get; set; }
        public int TabId { get; set; }

        public TextItemWithTab(string name, string fullName, Rect bounds, bool isEnabled, bool canExpand, IntPtr tabHandle, int tabId, AutomationElement tabItem)
            : base(name, fullName, bounds, isEnabled, canExpand)
        {
            TabItem = tabItem;
            TabHandle = tabHandle;
            TabId = tabId;
        }

        public override void Prepare()
        {
            if (TabHandle != IntPtr.Zero)
            {
                var id = TabId;
                Win32Interop.SendMessage(TabHandle, Win32Interop.TCM_SETCURFOCUS, new IntPtr(TabId), IntPtr.Zero);
            }
            else
            {
                TabItem.TrySelectItem();
            }
        }
    }
}
