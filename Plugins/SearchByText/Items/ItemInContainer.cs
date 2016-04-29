using System.Windows;
using System.Windows.Automation;
using System.Threading.Tasks;
using UIAssistant.Utility.Extensions;
using UIAssistant.Core.Input;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class ItemInContainer : SearchByTextItem
    {
        AutomationElement Element { get; set; }
        AutomationElement Container { get; set; }

        public ItemInContainer(string name, Rect bounds, bool isEnabled, AutomationElement element, AutomationElement container)
            : base(name, name, bounds, isEnabled, false)
        {
            Element = element;
            Container = container;
        }

        public override void Execute()
        {
            if (!IsEnabled)
            {
                return;
            }
            Task.Run(() =>
            {
                Element.ScrollIntoView();
                Element.TrySelectItem();

                var pt = MouseOperation.GetMousePosition();
                MouseOperation.Click(Element.Current.BoundingRectangle);
                MouseOperation.Move(pt);
            });
        }
    }
}
