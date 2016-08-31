using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Automation;

using UIAssistant.Core.Enumerators;
using UIAssistant.Core.Events;
using UIAssistant.Core.Input;
using UIAssistant.Utility.Extensions;
using UIAssistant.Plugin.SearchByText.Items;

namespace UIAssistant.Plugin.SearchByText.Enumerators.ForCommand
{
    class ContextMenu : AbstarctSearchForCommand
    {
        public AutomationElement ContextRoot { get; set; }
        public List<string> _expandableItems = new List<string>();

        bool _isCanceled = false;
        bool _isFinished = false;
        System.Windows.Point _mouseCurrentPosition;

        public override void Dispose()
        {
            _isCanceled = true;
            while (!_isFinished)
            {
                System.Threading.Thread.Sleep(100);
            }
            base.Dispose();
        }

        public override void Enumerate(HUDItemCollection results)
        {
            _mouseCurrentPosition = MouseOperation.GetMousePosition();
            MouseOperation.Move(0, 0);
            _results = results;
            GetMenuItems(ContextRoot, null, new List<AutomationElement>());
            _isFinished = true;
            CloseChildContextMenu();
            return;
        }

        private void CloseChildContextMenu()
        {
            Task.Run(() =>
            {
                var contextMenuBounds = ContextRoot.Current.BoundingRectangle;
                var menuTopCenter = contextMenuBounds.TopCenter();
                MouseOperation.Move(menuTopCenter.X, menuTopCenter.Y + 2);
                System.Threading.Thread.Sleep(400);
                if (!contextMenuBounds.Contains(_mouseCurrentPosition))
                {
                    MouseOperation.Move(_mouseCurrentPosition);
                }
            });
        }

        private bool GetMenuItems(AutomationElement element, string parent, List<AutomationElement> groups)
        {
            bool ret = false;
            var menuItems = element.FindAll(TreeScope.Children, Condition.TrueCondition).Cast<AutomationElement>();

            foreach (var item in menuItems)
            {
                if (_isCanceled)
                {
                    break;
                }

                try
                {
                    var elementInfo = item.Current;
                    ControlType elementType = elementInfo.ControlType;
                    if (elementType.IsThroughElement())
                    {
                        continue;
                    }

                    string addName = elementInfo.Name;
                    if (addName == null || addName == "")
                    {
                        continue;
                    }
                    string itemName = addName.Clone() as string;
                    string fullpath;
                    string shortcutKey;
                    bool canExpand;
                    if (FormatName(parent, elementType, item, ref addName, out fullpath, out shortcutKey, out canExpand))
                    {
                        if (item.CanExpandElement(elementType))
                        {
                            string ancestor = parent;
                            if (parent == null)
                            {
                                ancestor = itemName;
                            }
                            else
                            {
                                ancestor = parent + Consts.Delimiter + itemName;
                            }

                            if (item.IsWPF())
                            {
                                _expandableItems.Add(itemName);
                                item.TryExpand();
                                GetMenuItems(item, ancestor, new List<AutomationElement>());
                                item.TryCollapse();
                                _expandableItems.Remove(itemName);
                            }
                            else
                            {
                                var observer = new PopupObserver();
                                observer.Callback += x =>
                                {
                                    // HACK: デッドロックを避けるために、2箇所で Dispose しているけれど、微妙
                                    observer.Dispose();
                                    GetMenuItems(x, ancestor, new List<AutomationElement>());
                                    item.TryCollapse();
                                    _expandableItems.Remove(itemName);
                                };
                                _expandableItems.Add(itemName);
                                observer.Observe();
                                item.TryExpand();
                                observer.Wait();
                                observer.Dispose();
                            }
                        }
                        var result = new MenuItemUIA(itemName, fullpath, elementInfo.BoundingRectangle, elementInfo.IsEnabled, canExpand, item, ContextRoot, _expandableItems.ToArray().ToList());
                        _results.Add(result);
                    }

                    ret = true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Print("{0}", e.Message);
                }
            }
            Update();
            return ret;
        }
    }
}
