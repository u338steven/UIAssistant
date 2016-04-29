using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Automation;
using UIAssistant.Core.Enumerators;
using UIAssistant.Utility.Extensions;
using UIAssistant.Plugin.SearchByText.Items;

namespace UIAssistant.Plugin.SearchByText.Enumerators
{
    class SearchContainer : ISearchByTextEnumerator
    {
#pragma warning disable 414
        public event Action Updated;
        public event Action Finished;
#pragma warning restore 414

        public void Cancel()
        {
            Dispose();
        }

        public void Dispose()
        {
            Updated = null;
            Finished = null;
        }

        public void Enumerate(HUDItemCollection results)
        {
            EnumerateInternal(results);
            Finished?.Invoke();
            Finished = null;
        }

        private void EnumerateInternal(HUDItemCollection results)
        {
            var element = AutomationElement.FocusedElement;
            if (element == null)
            {
                return;
            }

            var info = element.Current;
            var type = info.ControlType;

            if (type.IsContainer())
            {
                if (type == ControlType.ComboBox)
                {
                    element.ExpandComboBox();
                }
                else
                {
                    element.TryExpand();
                }
            }
            else if (type.IsItem())
            {
                element = element.GetParent();
            }
            else
            {
                element = element.GetParent();
                info = element.Current;
                type = info.ControlType;
                if (!type.IsContainer())
                {
                    return;
                }
                if (type == ControlType.ComboBox)
                {
                    element.ExpandComboBox();
                }
            }

            var children = element?.FindAll(TreeScope.Descendants, Condition.TrueCondition).Cast<AutomationElement>();

            foreach (var child in children)
            {
                var childInfo = child.Current;
                if (!childInfo.ControlType.IsItem())
                {
                    continue;
                }
                var item = new ItemInContainer(childInfo.Name, childInfo.BoundingRectangle, childInfo.IsEnabled, child, element);
                results.Add(item);
            }
        }
    }
}
