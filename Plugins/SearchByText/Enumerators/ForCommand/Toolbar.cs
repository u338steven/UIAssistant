using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Automation;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Utility.Extensions;
using UIAssistant.Plugin.SearchByText.Items;

namespace UIAssistant.Plugin.SearchByText.Enumerators.ForCommand
{
    class Toolbar : AbstarctSearchForCommand
    {
        public override void Enumerate(ICollection<IHUDItem> results)
        {
            _results = results;

            AutomationElement element = AutomationElement.FromHandle(MainWindowHandle);
            GetToolbarItems(element);
            return;
        }

        private bool GetToolbarItems(AutomationElement element)
        {
            int retCounter = 0;
            PropertyCondition toolbarCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar);
            PropertyCondition paneCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane);
            PropertyCondition windowCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window);
            Condition propCondition = new OrCondition(toolbarCondition, paneCondition, windowCondition);

            var toolbars = element.FindAll(TreeScope.Children, propCondition).Cast<AutomationElement>();
            foreach (var el in toolbars)
            {
                ControlType type = el.Current.ControlType;

                if (type == ControlType.ToolBar)
                {
                    if (GetToolbarItem(el))
                    {
                        ++retCounter;
                    }
                }
                else
                {
                    if (GetToolbarItems(el))
                    {
                        ++retCounter;
                    }
                }
            }
            return retCounter > 0;
        }

        private bool GetToolbarItem(AutomationElement el)
        {
            bool ret = false;
            var toolbarItems = el.FindAll(TreeScope.Descendants, Condition.TrueCondition).Cast<AutomationElement>();

            var pt = SearchByText.UIAssistantAPI.MouseOperation.GetMousePosition();
            foreach (AutomationElement item in toolbarItems)
            {
                try
                {
                    var elementInfo = item.Current;
                    ControlType elementType = elementInfo.ControlType;
                    if (elementType.IsThroughElement())
                    {
                        continue;
                    }


                    SearchByText.UIAssistantAPI.MouseOperation.DoMouseEvent(elementInfo.BoundingRectangle);
                    string addName = elementInfo.Name;
                    if (addName == null || addName == "")
                    {
                        continue;
                    }
                    string itemName = addName.Clone() as string;
                    string fullpath;
                    string shortcutKey;
                    bool canExpand;
                    if (FormatName("Toolbar", elementType, item, ref addName, out fullpath, out shortcutKey, out canExpand))
                    {
                        var result = new ToolbarItem(itemName, fullpath, elementInfo.BoundingRectangle, item, elementInfo.IsEnabled, canExpand);
                        _results.Add(result);
                    }

                    ret = true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Print("{0}", e.Message);
                }
            }
            SearchByText.UIAssistantAPI.MouseOperation.Move(pt);
            return ret;
        }
    }
}
