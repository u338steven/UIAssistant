using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Automation;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.HitaHint.Enumerators
{
    class WidgetsInWindow : IWidgetEnumerator
    {
        #region EnumTargets
        public static readonly ControlType[] _enumerateTargets = {
            ControlType.Button,
            ControlType.Calendar,
            ControlType.CheckBox,
            ControlType.ComboBox,
            ControlType.Custom,
            ControlType.DataGrid,
            ControlType.DataItem,
            ControlType.Edit,
            ControlType.HeaderItem,
            ControlType.Hyperlink,
            ControlType.Image,
            ControlType.ListItem,
            ControlType.MenuItem,
            ControlType.Pane,
            ControlType.ProgressBar,
            ControlType.RadioButton,
            ControlType.Slider,
            ControlType.Spinner,
            ControlType.SplitButton,
            ControlType.TabItem,
            ControlType.Table,
            ControlType.Thumb,
            ControlType.TreeItem,
        };
        #endregion

        public void Enumerate(ICollection<IHUDItem> container)
        {
            HitaHint.UIAssistantAPI.GetWidgetEnumerator().Enumerate(container, null, _enumerateTargets);
        }

        public void Dispose()
        {
        }
    }
}
