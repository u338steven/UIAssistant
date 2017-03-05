using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Automation;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Utility;
using UIAssistant.Utility.Win32;

namespace UIAssistant.Plugin.HitaHint.Enumerators
{
    internal class UIAutomationEnumerator
    {
        private Condition Condition { get; set; }
        private HashSet<ControlType> IgnoreElements { get; set; }
        private HashSet<ControlType> EndElements { get; set; }
        private HashSet<ControlType> CollectionElements { get; set; }
        private HashSet<ControlType> ItemElements { get; set; }
        private System.Windows.Rect DesktopBounds { get; set; }
        private Queue<AutomationElement> _taskList = new Queue<AutomationElement>();

        public UIAutomationEnumerator()
        {
            DesktopBounds = Screen.Bounds;
            Condition = new PropertyCondition(AutomationElement.IsEnabledProperty, true);
            IgnoreElements = new HashSet<ControlType>();
            EndElements = new HashSet<ControlType>(
                new ControlType[] {
                    ControlType.Button,
                    ControlType.RadioButton,
                    ControlType.CheckBox,
                    ControlType.Hyperlink,
                    // ControlType.ListItem,
                    ControlType.MenuItem,
                    ControlType.SplitButton,
                    ControlType.ComboBox,
                    ControlType.Edit,
                    ControlType.Text,
                    ControlType.ScrollBar,
                    ControlType.Thumb,
                    ControlType.Slider,
                    ControlType.Image,
                });
            CollectionElements = new HashSet<ControlType>(
                new ControlType[] {
                    ControlType.List,
                    ControlType.Tab,
                    ControlType.Header,
                });
            ItemElements = new HashSet<ControlType>(
                new ControlType[] {
                    ControlType.ListItem,
                    ControlType.TabItem,
                    ControlType.HeaderItem
            }   );
        }

        private void AddHUDItem(ICollection<IHUDItem> container, System.Windows.Rect rect)
        {
            var item = new WidgetInfo(rect);
            item.Location = rect.Location;
            item.Bounds = rect;

            container.Add(item);
        }

        public void ChangeCondition(Condition condition)
        {
            Condition = condition;
        }

        public void AddIgnore(params ControlType[] types)
        {
            types.ForEach(item => IgnoreElements.Add(item));
        }

        public void CreateHUDItems(ICollection<IHUDItem> container, AutomationElement rootElement)
        {
            var element = rootElement;
            _taskList.Enqueue(rootElement);

            ControlType ct;
            System.Windows.Rect rect;
            AutomationElement.AutomationElementInformation aeInfo;
            IEnumerable<AutomationElement> children;

            while (_taskList.Count > 0)
            {
                element = _taskList.Dequeue();
                children = element.FindAll(TreeScope.Children, Condition)?.Cast<AutomationElement>();

                if (children == null)
                {
                    continue;
                }

                foreach (AutomationElement elementNode in children)
                {
                    // call .Current once, because of high cost
                    aeInfo = elementNode.Current;
                    ct = aeInfo.ControlType;

                    if (!IgnoreElements.Contains(ct))
                    {
                        rect = aeInfo.BoundingRectangle;
                        if (rect.Width != 0 && rect.Height != 0)
                        {
                            rect.X -= DesktopBounds.Left;
                            rect.Y -= DesktopBounds.Top;
                            AddHUDItem(container, rect);
                        }
                    }

                    if (CollectionElements.Contains(ct))
                    {
                        CreateHUDItemsForCollection(container, elementNode);
                        continue;
                    }

                    if (!EndElements.Contains(ct))
                    {
                        _taskList.Enqueue(elementNode);
                    }
                }
            }
        }

        // for List (except Tree)
        private void CreateHUDItemsForCollection(ICollection<IHUDItem> container, AutomationElement collection)
        {
            var bounds = collection.Current.BoundingRectangle;

            var treeWalker = new TreeWalker(Condition);
            var item = treeWalker.GetFirstChild(collection);

            var beginOnScreen = false;

            while (item != null)
            {
                var aeInfo = item.Current;
                var rect = aeInfo.BoundingRectangle;

                if (!IgnoreElements.Contains(aeInfo.ControlType) && bounds.Contains(rect))
                {
                    if (!beginOnScreen && ItemElements.Contains(aeInfo.ControlType))
                    {
                        beginOnScreen = true;
                    }
                    var ct = aeInfo.ControlType;
                    rect.X -= DesktopBounds.Left;
                    rect.Y -= DesktopBounds.Top;
                    AddHUDItem(container, rect);
                    if (!EndElements.Contains(ct))
                    {
                        _taskList.Enqueue(item);
                    }
                }
                else if (beginOnScreen)
                {
                    break;
                }

                item = treeWalker.GetNextSibling(item);
            }
        }

        public void Enumerate(ICollection<IHUDItem> container, AutomationElement root)
        {
            var results = new List<IHUDItem>();
            _taskList = new Queue<AutomationElement>();
            CreateHUDItems(results, root);
            results.Distinct(new HUDItemComparer()).Where(x => results.All(y => x.Bounds == y.Bounds || !x.Bounds.Contains(y.Bounds))).ToList().ForEach(x => container.Add(x));
        }

        public void Enumerate(ICollection<IHUDItem> container)
        {
            Enumerate(container, Win32Window.ActiveWindow?.Element);
        }
    }

    class HUDItemComparer : EqualityComparer<IHUDItem>
    {
        public override bool Equals(IHUDItem x, IHUDItem y)
        {
            return x.Bounds == y.Bounds;
        }

        public override int GetHashCode(IHUDItem obj)
        {
            return obj.Bounds.GetHashCode();
        }
    }
}
