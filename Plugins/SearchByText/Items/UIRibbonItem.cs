using System.Windows;
using System.Windows.Automation;
using System.Threading.Tasks;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class UIRibbonItem : SearchByTextItem
    {
        public AutomationElement Tab { get; set; }
        public AutomationElement Container { get; set; }

        public UIRibbonItem(string name, string fullName, Rect bounds, bool isEnabled, bool canExpand = false, AutomationElement tab = null, AutomationElement container = null)
            : base(name, fullName, bounds, isEnabled, canExpand)
        {
            Tab = tab;
            Container = container;
        }

        public override void Execute()
        {
            if (!IsEnabled)
            {
                return;
            }
            var t = Task.Run(() =>
            {
                Tab?.TryDoDefaultAction();
                var target = GetCurrentElement(Container);
                target?.DoAction();
            });
        }
    }
}
