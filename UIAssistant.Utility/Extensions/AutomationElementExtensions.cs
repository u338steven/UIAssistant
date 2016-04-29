using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Automation;
using UIAssistant.Utility.Win32;

namespace UIAssistant.Utility.Extensions
{
    public static class AutomationElementExtensions
    {
        public static AutomationElement GetParent(this AutomationElement el)
        {
            if (el == null)
            {
                return null;
            }
            return TreeWalker.ContentViewWalker.GetParent(el);
            //return TreeWalker.ControlViewWalker.GetParent(el);
            //return TreeWalker.RawViewWalker.GetParent(el);
        }

        [Flags]
        private enum AccessibleSelection : int
        {
            None = 0,
            TakeFocus = 0x1,
            TakeSelection = 0x2,
            ExtendSelection = 0x4,
            AddSelection = 0x8,
            RemoveSelection = 0x10,
        }

        public static void TrySetFocusLegacy(this AutomationElement element)
        {
            object selectionItemPattern;
            if (element.TryGetCurrentPattern(LegacyIAccessiblePattern.Pattern, out selectionItemPattern))
            {
                try
                {
                    ((LegacyIAccessiblePattern)selectionItemPattern).Select((int)AccessibleSelection.TakeSelection);
                }
                catch
                {
                }
            }
        }

        public static int GetTabCount(this AutomationElement el)
        {
            return Win32Interop.SendMessage(el.Current.NativeWindowHandle, Win32Interop.TCM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero).ToInt32();
        }

        public static int GetTabCurrentIndex(this AutomationElement el)
        {
            return Win32Interop.SendMessage(el.Current.NativeWindowHandle, Win32Interop.TCM_GETCURFOCUS, IntPtr.Zero, IntPtr.Zero).ToInt32();
        }

        public static bool TrySelectTab(this AutomationElement el, int index)
        {
            // window handle を取得できなかったら、たぶん WPF
            var handle = el.Current.NativeWindowHandle;
            if (el.Current.NativeWindowHandle == IntPtr.Zero)
            {
                return false;
            }
            Win32Interop.SendMessage(handle, Win32Interop.TCM_SETCURFOCUS, new IntPtr(index), IntPtr.Zero);
            return true;
        }

        public static bool TrySelectItem(this AutomationElement el)
        {
            object selectionItem;
            if (el.TryGetCurrentPattern(SelectionItemPattern.Pattern, out selectionItem))
            {
                try
                {
                    SelectionItemPattern item = (SelectionItemPattern)selectionItem;
                    item.Select();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public static bool TryDoDefaultAction(this AutomationElement el)
        {
            if (!el.Current.IsEnabled)
            {
                return false;
            }
            object invoke;
            if (el.TryGetCurrentPattern(InvokePattern.Pattern, out invoke))
            {
                ((InvokePattern)invoke).Invoke();
                return true;
            }

            object legacyPattern;
            if (el.TryGetCurrentPattern(LegacyIAccessiblePattern.Pattern, out legacyPattern))
            {
                LegacyIAccessiblePattern legacy = (LegacyIAccessiblePattern)legacyPattern;
                if (legacy.Current.DefaultAction != "")
                {
                    legacy.DoDefaultAction();
                }
                return true;
            }

            return false;
        }

        public static string GetShortcutKey(this AutomationElement el)
        {
            string ret = el.Current.AcceleratorKey;

            if (ret != null && ret != "")
            {
                return ret;
            }
            return "";
        }

        public static string GetAceessKeys(this AutomationElement el)
        {
            return el.Current.AccessKey;
        }

        public static bool CanExpandElement(this AutomationElement el, ControlType elementType)
        {
            if (elementType == ControlType.SplitButton || elementType == ControlType.Button || elementType == ControlType.MenuItem)
            {
                object ecPattern;
                if (el.TryGetCurrentPattern(LegacyIAccessiblePattern.Pattern, out ecPattern))
                {
                    uint state = ((LegacyIAccessiblePattern)ecPattern).Current.State;
                    if ((state & 0x40000000) != 0)
                    {
                        return true;
                    }
                }
            }
            else if (elementType == ControlType.ComboBox)
            {
                object ecPattern;
                if (el.TryGetCurrentPattern(LegacyIAccessiblePattern.Pattern, out ecPattern))
                {
                    uint state = ((LegacyIAccessiblePattern)ecPattern).Current.State;
                    if ((state & 0x400) != 0 || (state & 0x200) != 0)
                    {
                        return true;
                    }
                }
                var buttonCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button);
                var button = el.FindFirst(TreeScope.Children, buttonCondition);
                if (button != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsSelected(this AutomationElement el)
        {
            object legacyPattern;
            if (el.TryGetCurrentPattern(LegacyIAccessiblePattern.Pattern, out legacyPattern))
            {
                if ((((LegacyIAccessiblePattern)legacyPattern).Current.State & 2) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsGroup(this AutomationElement el)
        {
            if (el.Current.ClassName == "TGroupBox")
            {
                return true;
            }
            object legacyPattern;
            if (el.TryGetCurrentPattern(LegacyIAccessiblePattern.Pattern, out legacyPattern))
            {
                if (((LegacyIAccessiblePattern)legacyPattern).Current.Role == 0x14)
                {
                    return true;
                }
            }
            return false;
        }

        public static void ExpandComboBox(this AutomationElement el)
        {
            if (!el.TryExpand())
            {
                el.DoAction();
            }
        }

        public static void CollapseComboBox(this AutomationElement el)
        {
            if (!el.TryCollapse())
            {
                el.DoAction();
            }
        }

        public static bool IsComboBox(this AutomationElement el)
        {
            if (el.Current.ControlType == ControlType.ComboBox)
            {
                return true;
            }

            var element = el.GetParent();
            var info = element.Current;
            var type = info.ControlType;
            if (type == ControlType.ComboBox)
            {
                return true;
            }
            return false;
        }

        public static bool TryExpand(this AutomationElement el)
        {
            object expand;
            if (el.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out expand))
            {
                ((ExpandCollapsePattern)expand).Expand();
                return true;
            }
            return false;
        }

        public static bool TryCollapse(this AutomationElement el)
        {
            object expand;
            if (el.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out expand))
            {
                ((ExpandCollapsePattern)expand).Collapse();
                return true;
            }
            return false;
        }

        public static void DoAction(this AutomationElement el)
        {
            if (el.Current.ControlType == ControlType.ComboBox)
            {
                var buttonCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button);
                var button = el.FindFirst(TreeScope.Children, buttonCondition);
                if (button != null)
                {
                    button.TryDoDefaultAction();
                }
            }
            else
            {
                el.TryDoDefaultAction();
            }
        }

        public static void ScrollIntoView(this AutomationElement el)
        {
            object scrollItemPattern;
            if (el.TryGetCurrentPattern(ScrollItemPattern.Pattern, out scrollItemPattern))
            {
                // デスクトップの検索時に Exception になるため try catch
                try
                {
                    ((ScrollItemPattern)scrollItemPattern).ScrollIntoView();
                }
                catch
                {
                }
            }
        }

        public static string GetAncestors(this AutomationElement descendant, int count = 10)
        {
            var ancestor = TreeWalker.ContentViewWalker.GetParent(descendant);
            if (ancestor != null && ancestor != AutomationElement.RootElement && count >= 0)
            {
                var name = descendant.Current.Name;
                if (name == null || (name == ""))
                {
                    return ancestor.GetAncestors(count);
                }
                else
                {
                    return ancestor.GetAncestors(--count) + " | " + name;
                }
            }
            return "";
        }

        static HashSet<ControlType> _ignoreFocusElements = new HashSet<ControlType> { ControlType.Tab, ControlType.ToolBar, ControlType.List, ControlType.Group, ControlType.Tree, ControlType.DataGrid, ControlType.Table, ControlType.Window, ControlType.Pane };
        public static bool IsIgnoreFocus(this ControlType type)
        {
            return _ignoreFocusElements.Contains(type);
        }

        static HashSet<ControlType> _containerElements = new HashSet<ControlType> { ControlType.Tab, ControlType.ToolBar, ControlType.List, ControlType.Group, ControlType.Custom, ControlType.Tree, ControlType.ComboBox, ControlType.DataGrid, ControlType.Table };
        public static bool IsContainer(this ControlType type)
        {
            return _containerElements.Contains(type);
        }

        static HashSet<ControlType> _itemElements = new HashSet<ControlType> { ControlType.TabItem, ControlType.ListItem, ControlType.Custom, ControlType.TreeItem, ControlType.DataItem };
        public static bool IsItem(this ControlType type)
        {
            return _itemElements.Contains(type);
        }

        static HashSet<ControlType> _throughElements = new HashSet<ControlType> { ControlType.Pane, ControlType.Window, ControlType.Tab, ControlType.ToolBar, ControlType.List, ControlType.Text, ControlType.Separator, ControlType.Group, ControlType.Custom, ControlType.Thumb };
        public static bool IsThroughElement(this ControlType type)
        {
            return _throughElements.Contains(type);
        }

        //        static HashSet<ControlType> _groupElements = new HashSet<ControlType> { ControlType.Pane, ControlType.Window, ControlType.Tab, ControlType.ToolBar, ControlType.List, ControlType.Group, ControlType.Header };
        static HashSet<ControlType> _groupElements = new HashSet<ControlType> { ControlType.Pane, ControlType.Window, ControlType.List, ControlType.Group, ControlType.Header, ControlType.Tab, ControlType.Tree, ControlType.Table, ControlType.DataGrid, ControlType.Document };
        //static HashSet<ControlType> _groupElements = new HashSet<ControlType> { ControlType.Pane, ControlType.Window, ControlType.List, ControlType.Group, ControlType.Header, ControlType.Tree, ControlType.Table, ControlType.DataGrid, ControlType.Document };
        //        static HashSet<ControlType> _groupElements = new HashSet<ControlType> { ControlType.Pane, ControlType.Window };
        public static bool IsGroupElement(this ControlType type)
        {
            return _groupElements.Contains(type);
        }

        static HashSet<ControlType> _textElements = new HashSet<ControlType> { ControlType.Text, ControlType.Document, ControlType.CheckBox, ControlType.Button, ControlType.HeaderItem, /*ControlType.TabItem,*/ ControlType.ListItem, ControlType.TreeItem, ControlType.RadioButton, ControlType.ComboBox, ControlType.Hyperlink, ControlType.Custom, ControlType.DataItem };
        public static bool hasText(this ControlType type)
        {
            return _textElements.Contains(type);
        }

        /*
                static HashSet<ControlType> _ignoreElements = new HashSet<ControlType> { ControlType.ScrollBar };
                static public bool IsIgnoreElement(ControlType type)
                {
                    if (_ignoreElements.Contains(type))
                    {
                        return true;
                    }
                    return false;
                }
        */
    }
}
