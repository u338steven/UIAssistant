using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using UIAssistant.Utility.Extensions;
using UIAssistant.Core.Input;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class MenuItemForWPF : SearchByTextItem
    {
        public AutomationElement[] Ancestors { get; set; }

        public MenuItemForWPF(string name, string fullName, Rect bounds, bool isEnabled, bool canExpand = false, AutomationElement[] ancestors = null)
            : base(name, fullName, bounds, isEnabled, canExpand)
        {
            Ancestors = ancestors;
        }

        public override void Prepare()
        {
        }

        public override void Execute()
        {
            if (!IsEnabled)
            {
                return;
            }
            Prepare();

            Task.Run(() =>
            {
                if (Ancestors.Length > 0)
                {
                    foreach (var ancestor in Ancestors)
                    {
                        ancestor.TryDoDefaultAction();
                    }
                    var element = GetCurrentElement(Ancestors[Ancestors.Length - 1]);
                    if (element == null)
                    {
                        UIAssistantAPI.NotifyWarnMessage("Search by Text", "Cannot run selected menu item");
                        return;
                    }
                    element.DoAction();
                }
            });
        }
    }
}
