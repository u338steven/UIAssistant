using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Windows.Automation;

using UIAssistant.Utility.Win32;

namespace UIAssistant.Core.Enumerators
{
    public class WidgetEnumerator : IHUDItemEnumerator
    {
        #region readonly
        static readonly ControlType[] allControlType = {
            ControlType.Button,
            ControlType.Calendar,
            ControlType.CheckBox,
            ControlType.ComboBox,
            ControlType.Custom,
            ControlType.DataGrid,
            ControlType.DataItem,
            ControlType.Document,
            ControlType.Edit,
            ControlType.Group,
            ControlType.Header,
            ControlType.HeaderItem,
            ControlType.Hyperlink,
            ControlType.Image,
            ControlType.List,
            ControlType.ListItem,
            ControlType.Menu,
            ControlType.MenuBar,
            ControlType.MenuItem,
            ControlType.Pane,
            ControlType.ProgressBar,
            ControlType.RadioButton,
            ControlType.ScrollBar,
            ControlType.Separator,
            ControlType.Slider,
            ControlType.Spinner,
            ControlType.SplitButton,
            ControlType.StatusBar,
            ControlType.Tab,
            ControlType.TabItem,
            ControlType.Table,
            ControlType.Text,
            ControlType.Thumb,
            ControlType.TitleBar,
            ControlType.ToolBar,
            ControlType.ToolTip,
            ControlType.Tree,
            ControlType.TreeItem,
            ControlType.Window,
        };
        #endregion

        private UIAutomationEnumerator enumerator;

        public WidgetEnumerator()
        {
            enumerator = new UIAutomationEnumerator();
        }

        private ControlType[] Invert(params ControlType[] types)
        {
            return allControlType.Except(types).ToArray();
        }

        public void Enumerate(HUDItemCollection container, Condition condition = null, params ControlType[] types)
        {
            SetCondition(condition);
            SetTypes(types);
            enumerator.Enumerate(container);
        }

        public void Retry(HUDItemCollection container)
        {
            enumerator.Enumerate(container);
        }

        private void SetCondition(Condition condition)
        {
            if (condition != null)
            {
                enumerator.ChangeCondition(condition);
            }
        }

        private void SetTypes(params ControlType[] types)
        {
            var t = Invert(types);
            enumerator.AddIgnore(t);
        }

        public void Enumerate(HUDItemCollection container, Win32Window root, Condition condition, params ControlType[] types)
        {
            SetCondition(condition);
            SetTypes(types);
            enumerator.Enumerate(container, root.Element);
        }
    }
}
