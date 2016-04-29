using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using System.Windows.Automation;

using UIAssistant.Utility.Win32;
using UIAssistant.Utility.Extensions;
using UIAssistant.Core.Enumerators;

namespace UIAssistant.Plugin.SearchByText.Items
{
    abstract class SearchByTextItem : IHUDItem
    {
        public string InternalText { get; set; }
        public string DisplayText { get; set; }
        public Point Location { get; set; }
        public Rect Bounds { get; set; }
        public ImageSource Image { get; set; }

        public bool IsEnabled { get; set; }
        public bool CanExpand { get; set; }
        public int ColoredStart { get; set; }
        public int ColoredLength { get; set; }

        public SearchByTextItem(IHUDItem item)
        {
            InternalText = item.InternalText;
            DisplayText = item.DisplayText;
            Location = item.Location;
            Bounds = item.Bounds;
            Image = item.Image;
        }

        public SearchByTextItem(string name, string fullName, Rect bounds, bool isEnabled, bool canExpand = false)
        {
            InternalText = name;
            DisplayText = fullName;
            Bounds = bounds;
            IsEnabled = isEnabled;
            CanExpand = canExpand;
        }

        public AutomationElement GetCurrentElement(AutomationElement root)
        {
            return GetCurrentElement(root, AutomationElement.NameProperty); ;
        }

        public AutomationElement GetCurrentElement(AutomationElement root, AutomationProperty property)
        {
            var propCondition = new PropertyCondition(property, InternalText);
            var candidate = root.FindAll(TreeScope.Descendants, propCondition);

            if (candidate.Count <= 0)
            {
                return null;
            }

            if (candidate.Count == 1)
            {
                return candidate[0];
            }

            foreach (AutomationElement element in candidate)
            {
                if (Bounds == element.Current.BoundingRectangle)
                {
                    return element;
                }
            }
            return null;
        }

        public virtual void Prepare()
        {

        }

        public virtual void Execute()
        {
            Prepare();
            var from = Win32Window.ActiveWindow.Bounds.ToClientCoordinate();
            Rect to = new Rect();
            AutomationElement element = null;
            Task.Run(() =>
            {
                element = GetCurrentElement(Win32Window.ActiveWindow.Element);
                if (IsEnabled && element != null)
                {
                    var parent = element.GetParent();
                    if (parent != null && parent.Current.ControlType.IsContainer())
                    {
                        parent.SetFocus();
                    }
                    element.ScrollIntoView();
                    var current = element.Current;
                    if (current.IsKeyboardFocusable)
                    {
                        try
                        {
                            if (!element.TrySelectItem())
                            {
                                element.SetFocus();
                            }
                            UIAssistantAPI.TopMost = true;
                        }
                        catch
                        {
                        }
                    }
                    to = current.BoundingRectangle;
                }
                else
                {
                    to = Bounds;
                }
                UIAssistantAPI.ScaleIndicatorAnimation(from, to);
                UIAssistantAPI.FlashIndicatorAnimation(to);
            });
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            return DisplayText.GetHashCode();
        }
    }
}
