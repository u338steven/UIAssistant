using System.Windows;
using System.Windows.Automation;
using System.Threading.Tasks;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class ToolbarItem : SearchByTextItem
    {
        public AutomationElement Item { get; set; }
        public ToolbarItem(string name, string fullName, Rect bounds, AutomationElement item, bool isEnabled, bool canExpand = false)
            : base(name, fullName, bounds, isEnabled, canExpand)
        {
            Item = item;
        }

        public override void Execute()
        {
            if (!IsEnabled)
            {
                return;
            }
            var t = Task.Run(() =>
            {
                Item?.DoAction();
            });
        }
    }
}
