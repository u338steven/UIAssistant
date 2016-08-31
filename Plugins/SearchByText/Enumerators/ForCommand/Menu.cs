using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using UIAssistant.Plugin.SearchByText.Items;
using UIAssistant.Core.Enumerators;

namespace UIAssistant.Plugin.SearchByText.Enumerators.ForCommand
{
    class Menu : AbstarctSearchForCommand
    {
        #region Win32Interop
        const int MFS_DISABLED = 0x00000003;
        const int MIIM_STATE = 0x00000001;
        const int MIIM_ID = 0x00000002;
        const int MIIM_SUBMENU = 0x00000004;
        const int MIIM_STRING = 0x00000040;

        [DllImport("user32.dll")]
        static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern bool GetMenuItemInfo(IntPtr hMenu, int uItem, bool fByPosition, [In, Out] MENUITEMINFO lpmii);

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
        public class MENUITEMINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MENUITEMINFO));
            public int fMask;
            public int fType;
            public int fState;
            public int wID;
            public IntPtr hSubMenu;
            public IntPtr hbmpChecked;
            public IntPtr hbmpUnchecked;
            public IntPtr dwItemData;
            public IntPtr dwTypeData;
            public int cch;
        }
        #endregion

        public override void Enumerate(HUDItemCollection results)
        {
            _results = results;
            var hMenu = GetMenu(MainWindowHandle);

            if (hMenu == IntPtr.Zero)
            {
                return;
            }

            GetChildren(hMenu, null);
            return;
        }

        private void GetMenuItemInfo(IntPtr hMenu, int index, ref MENUITEMINFO itemInfo, out string itemName)
        {
            itemInfo.dwTypeData = IntPtr.Zero;
            GetMenuItemInfo(hMenu, index, true, itemInfo);

            // From MSDN
            ++itemInfo.cch;
            itemInfo.dwTypeData = Marshal.AllocHGlobal(itemInfo.cch * sizeof(char));
            GetMenuItemInfo(hMenu, index, true, itemInfo);

            itemName = Marshal.PtrToStringUni(itemInfo.dwTypeData);
            Marshal.FreeHGlobal(itemInfo.dwTypeData);
        }

        private void GetChildren(IntPtr hMenu, string parent)
        {
            int count = GetMenuItemCount(hMenu);

            MENUITEMINFO itemInfo = new MENUITEMINFO();
            itemInfo.fMask = MIIM_STRING | MIIM_STATE | MIIM_SUBMENU | MIIM_ID;

            for (int i = 0; i < count; ++i)
            {
                string itemName;
                GetMenuItemInfo(hMenu, i, ref itemInfo, out itemName);

                if (string.IsNullOrEmpty(itemName))
                {
                    continue;
                }

                if (parent != null)
                {
                    itemName = parent + Consts.Delimiter + itemName;
                }
                itemName = itemName.Replace("&", "");

                if(itemInfo.hSubMenu == IntPtr.Zero)
                {
                    var isEnabled = (itemInfo.fState & MFS_DISABLED) == 0;
                    var id = itemInfo.wID;

                    var result = new MenuItem(itemName, isEnabled, id, MainWindowHandle);
                    _results.Add(result);
                    continue;
                }
                GetChildren(itemInfo.hSubMenu, itemName);
            }
        }
    }
}
