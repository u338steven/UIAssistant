using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows.Automation;
using System.Reactive.Linq;

using UIAssistant.Interfaces;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Plugin.SearchByText.Items;

namespace UIAssistant.Plugin.SearchByText.Enumerators.ForCommand
{
    class RibbonUI : AbstarctSearchForCommand
    {
        List<AutomationElement> _tabPane = new List<AutomationElement>();
        AutomationElement _initialPane = null;
        List<IDisposable> _disposables = new List<IDisposable>();

        public override void Enumerate(ICollection<IHUDItem> results)
        {
            _results = results;
            IntPtr ribbonRootHandle = IntPtr.Zero;
            IntPtr ribbonHandle = IntPtr.Zero;

            IWindow ribbonRoot = null;
            IWindow ribbon = null;

            FindRibbonUI(ref ribbonRoot, ref ribbon);
            if (ribbon != null)
            {
                PrepareUIAutomation(ribbon);
                // _initialPane があれば、RibbonUI は表示されている
                if (_initialPane != null)
                {
                    GetRibbonItems(ribbon.Element, false);
                }
                else
                {
                    var parent = SearchByText.UIAssistantAPI.ActiveWindow;
                    GetRibbonItems(parent.Element, true);
                }
            }
        }

        private void GetRibbonItems(AutomationElement ribbonWindow, bool isOffScreen)
        {
            PropertyCondition paneCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane);
            var ribbonBelows = ribbonWindow.FindAll(TreeScope.Children, paneCondition);
            var currentPane = _initialPane;
            if (ribbonBelows.Count == 0)
            {
                return;
            }
            AutomationElement ribbonBelow;
            if (isOffScreen)
            {
                try
                {
                    // 隠れている リボン UI を無理矢理取得する
                    _initialPane = _tabPane[0];
                    _initialPane.TryDoDefaultAction();
                    _initialPane.TrySelectItem();
                    var notNullName = new NotCondition(new PropertyCondition(AutomationElement.NameProperty, ""));
                    ribbonBelow = ribbonBelows[0];
                    ribbonBelow = ribbonBelow.FindFirst(TreeScope.Descendants, notNullName);
                    ribbonBelow = ribbonBelow.FindFirst(TreeScope.Descendants, notNullName);
                    ribbonBelow = ribbonBelow.FindFirst(TreeScope.Descendants, notNullName);
                    ribbonBelow = ribbonBelow.FindFirst(TreeScope.Children, notNullName);
                    ribbonBelow = ribbonBelow.FindFirst(TreeScope.Children, Condition.TrueCondition);
                }
                catch
                {
                    return;
                }
            }
            else
            {
                ribbonBelow = ribbonBelows.Cast<AutomationElement>().First(el => el.Current.NativeWindowHandle == IntPtr.Zero);
            }
            if (ribbonBelow == null)
            {
                return;
            }
            try
            {
                if (!isOffScreen)
                {
                    _initialPane.TrySelectItem();
                    GetElements(ribbonBelow, false, null, _initialPane, ribbonBelow);
                }

                _tabPane.ForEach(el =>
                {
                    if (_isCancel)
                    {
                        return;
                    }
                    if (el.TryDoDefaultAction())
                    {
                        SearchByText.UIAssistantAPI.TopMost = true;
                        if (!isOffScreen)
                        {
                            GetElements(ribbonBelow, false, null, el, ribbonBelow);
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(100);
                            var cond = new PropertyCondition(AutomationElement.NameProperty, el.Current.Name);
                            var next = SearchByText.UIAssistantAPI.ActiveWindow.LastActivePopup.Element;

                            var elem = next?.FindFirst(TreeScope.Children | TreeScope.Descendants, cond);
                            if (elem == null)
                            {
                                return;
                            }
                            GetElements(elem, false, el.Current.Name, el, ribbonWindow);
                            currentPane = el;
                        }
                    }
                });
            }
            finally
            {
                if (!isOffScreen)
                {
                    _initialPane.TryDoDefaultAction();
                }
                else
                {
                    // Close
                    currentPane?.TryDoDefaultAction();
                }
            }
        }
        bool _isCancel = false;

        private void FindRibbonUI(ref IWindow ribbonRoot, ref IWindow ribbon)
        {
            ribbonRoot = SearchByText.UIAssistantAPI.ActiveWindow.FindChild("UIRibbonCommandBarDock");
            while (ribbonRoot.WindowHandle != IntPtr.Zero)
            {
                ribbonRoot = SearchByText.UIAssistantAPI.ActiveWindow.FindChild(ribbonRoot, "UIRibbonCommandBarDock");
                ribbon = ribbonRoot?.FindChild("UIRibbonCommandBar")
                    .FindChild("UIRibbonWorkPane").FindChild("NUIPane").FindChild("NetUIHWND");
                if (ribbon != null && ribbon.WindowHandle != IntPtr.Zero)
                {
                    break;
                }
            }
        }

        private bool PrepareUIAutomation(IWindow ribbon)
        {
            PropertyCondition propCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Tab);
            var ribbonRoot = ribbon.Element;
            var element = ribbonRoot.FindFirst(TreeScope.Descendants, propCondition);
            if (element == null)
            {
                return false;
            }

            GetElements(ribbonRoot, true, null, null, ribbonRoot);

            return true;
        }

        private void GetElements(AutomationElement element, bool root, string parent, AutomationElement group, AutomationElement ribbonRootElement)
        {
            PropertyCondition propCondition = new PropertyCondition(AutomationElement.IsContentElementProperty, true);
            var col = element.FindAll(TreeScope.Children, propCondition).Cast<AutomationElement>();
            foreach (var el in col)
            {
                try
                {
                    var elementInfo = el.Current;
                    string addName = elementInfo.Name;
                    ControlType elementType = elementInfo.ControlType;
                    // コンボボックスのエディットボックスが、リボン UI のウィンドウ直下に表示されるときがあるので、それを除外する
                    if (elementType == ControlType.Pane)
                    {
                        if (elementInfo.NativeWindowHandle != IntPtr.Zero)
                        {
                            return;
                        }
                    }

                    if (addName != null && addName.Length != 0)
                    {
                        string itemName = addName.Clone() as string;
                        string fullpath;
                        string shortcutKey;
                        bool canExpand;
                        if (FormatName(parent, elementType, el, ref addName, out fullpath, out shortcutKey, out canExpand))
                        {
                            if (shortcutKey == null || shortcutKey == "")
                            {
                                shortcutKey = el.GetAceessKeys();
                            }

                            //System.Diagnostics.Debug.Print($"{fullpath}");
                            var result = new UIRibbonItem(itemName, fullpath, elementInfo.BoundingRectangle, elementInfo.IsEnabled, canExpand, group, ribbonRootElement);
                            _results.Add(result);
                        }
                    }
                    else
                    {
                        addName = parent;
                    }

                    if (elementType == ControlType.ComboBox)
                    {
                        return;
                    }

                    bool tmpRoot = root;
                    if (root && elementType == ControlType.Pane)
                    {
                        return;
                    }
                    if (root && elementType == ControlType.TabItem)
                    {
                        if (el.IsSelected())
                        {
                            _initialPane = el;
                        }
                        else
                        {
                            tmpRoot = false;
                            _tabPane.Add(el);
                        }
                    }

                    AutomationElement rootElement;
                    if (ribbonRootElement == null)
                    {
                        rootElement = el;
                    }
                    else
                    {
                        rootElement = ribbonRootElement;
                    }
                    if (group == null && _initialPane != null)
                    {
                        GetElements(el, tmpRoot, addName, _initialPane, rootElement);
                    }
                    else
                    {
                        GetElements(el, tmpRoot, addName, group, rootElement);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Print("GetMenus: {0}", e.Message);
                }
            }
        }

        public override void Dispose()
        {
            _isCancel = true;
            _disposables?.ForEach(x => x.Dispose());
            _disposables = null;
            base.Dispose();
        }
    }
}
