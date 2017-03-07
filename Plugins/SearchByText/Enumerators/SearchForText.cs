using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Windows.Automation;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

using UIAssistant.Interfaces.HUD;
using UIAssistant.Plugin.SearchByText.Items;

namespace UIAssistant.Plugin.SearchByText.Enumerators
{
    class TabGroup
    {
        public AutomationElement Root { get; private set; }
        public List<AutomationElement> TabItems { get; private set; } = new List<AutomationElement>();

        public int InitialIndex { get; private set; }
        public AutomationElement InitialTab { get; private set; }

        public TabGroup(AutomationElement root)
        {
            Root = root;
        }

        public void Add(AutomationElement tabItem)
        {
            TabItems.Add(tabItem);
        }

        public void MarkInitial(AutomationElement initial)
        {
            InitialIndex = Root.GetTabCurrentIndex();
            InitialTab = initial;
        }
    }

    class SearchForText : ISearchByTextEnumerator
    {
        public event EventHandler Updated;
        public event EventHandler Finished;
        Queue<AutomationElement> _groups = new Queue<AutomationElement>(100);
        ICollection<IHUDItem> _results;
        List<IDisposable> _disposables = new List<IDisposable>();
        bool _isCanceled = false;

        public void Cancel()
        {
            _isCanceled = true;
            Dispose();
        }
        
        public void Dispose()
        {
            _disposables?.ForEach(x => x.Dispose());
            _disposables = null;
            Updated = null;
            Finished = null;
        }

        public void Enumerate(ICollection<IHUDItem> results)
        {
            _results = results;
            EnumerateInternal();
        }

        private void EnumerateInternal()
        {
            var root = SearchByText.UIAssistantAPI.ActiveWindow.Element;
            var tabGroups = GetTabGroups(root);

            if (tabGroups.Count == 0)
            {
                GetElements(root);
                Updated?.Invoke(this, EventArgs.Empty);
                Finished?.Invoke(this, EventArgs.Empty);
                return;
            }

            tabGroups.ToObservable().ForEachAsync(async tabGroup =>
            {
                await Task.Run(() =>
                {
                    var tab = tabGroup.Root;
                    var tabContainer = tab.GetParent();
                    int tabCount = 0;

                    tabGroup.TabItems.ForEach(tabItem =>
                    {
                        if (_isCanceled)
                        {
                            return;
                        }
                        if (!tab.TrySelectTab(tabCount))
                        {
                            // For WPF
                            tabItem.TrySelectItem();
                            tabContainer = tabItem;
                        }
                        var parents = tabItem.Current.Name;
                        GetGroup(tabContainer);
                        var tabHandle = tab.Current.NativeWindowHandle;
                        EnumerateElements(tabContainer, parents, tabHandle, tabCount, tabItem);
                        while (_groups.Count > 0)
                        {
                            var element = _groups.Dequeue();
                            GetGroupElement(element, parents, tabHandle, tabCount, tabItem);
                        }
                        ++tabCount;
                        Updated?.Invoke(this, EventArgs.Empty);
                    });

                    var rootHandle = root.Current.NativeWindowHandle;
                    EnumerateElements(root, null, rootHandle, tabCount, null);
                    while (_groups.Count > 0)
                    {
                        var element = _groups.Dequeue();
                        GetGroupElement(element, null, rootHandle, tabCount, null);
                    }
                    if (!tab.TrySelectTab(tabGroup.InitialIndex))
                    {
                        tabGroup.InitialTab?.TrySelectItem();
                    }
                    Updated?.Invoke(this, EventArgs.Empty);
                    Finished?.Invoke(this, EventArgs.Empty);
                });
            }).Tap(x => _disposables?.Add(x));
        }

        private void GetElements(AutomationElement root)
        {
            GetGroup(root);
            var rootHandle = root.Current.NativeWindowHandle;
            EnumerateElements(root, null, rootHandle, 0, null);
            while (_groups.Count > 0)
            {
                var element = _groups.Dequeue();
                GetGroupElement(element, null, rootHandle, 0, null);
            }
        }

        private void GetGroupElement(AutomationElement element, string parents, IntPtr tabHandle, int tabId, AutomationElement tabItem)
        {
            EnumerateElements(element, parents, tabHandle, tabId, tabItem);
        }

        List<AutomationElement> _gr = new List<AutomationElement>();
        private void GetGroup(AutomationElement element)
        {
            _gr.Clear();
            Condition pane = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane);
            Condition group = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Group);
            Condition cond = new OrCondition(pane, group);
            var col = element.FindAll(TreeScope.Descendants, cond).Cast<AutomationElement>();

            _gr = col.Where(el => el.IsGroup()).ToList();
        }

        private void EnumerateElements(AutomationElement container, string parents, IntPtr tabHandle, int tabId, AutomationElement tabItem)
        {
            Condition cond = new PropertyCondition(AutomationElement.IsContentElementProperty, true);
            var children = container?.FindAll(TreeScope.Children, cond).Cast<AutomationElement>();

            foreach (var child in children)
            {
                if (_isCanceled)
                {
                    return;
                }
                var childInfo = child.Current;
                var elementType = childInfo.ControlType;
                if (elementType.IsGroupElement())
                {
                    _groups.Enqueue(child);
                }
                else if (elementType.hasText())
                {
                    string name = childInfo.Name;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = childInfo.LabeledBy?.Current.Name;
                        if (string.IsNullOrEmpty(name))
                            continue;
                    }
                    var rect = childInfo.BoundingRectangle;

                    var localparent = _gr.FirstOrDefault(groupElement => groupElement.Current.BoundingRectangle.Contains(rect))?.Current.Name;
                    var localparents = FormatName(parents, localparent);

                    string fullName = FormatName(localparents, name);

                    bool canExpand = false;
                    if (elementType == ControlType.ComboBox)
                    {
                        fullName += Consts.Expandable;
                        canExpand = true;
                    }

                    var result = new TextItemWithTab(name, fullName, childInfo.BoundingRectangle, childInfo.IsEnabled, canExpand, tabHandle, tabId, tabItem);
                    _results.Add(result);
                }
            }

            return;
        }

        public static string FormatName(string parents, string name)
        {
            if (name == null)
            {
                return parents;
            }
            var replacedName = Regex.Replace(name, @"[\r\n\t]", "");
            if (name == "")
            {
                return parents;
            }
            if (parents != null)
            {
                return parents + Consts.Delimiter + replacedName;
            }
            return replacedName;
        }

        private List<TabGroup> GetTabGroups(AutomationElement element)
        {
            List<TabGroup> tabGroups = new List<TabGroup>();
            Condition cond = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);
            var condTab = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Tab);
            var tabs = element.FindAll(TreeScope.Descendants, condTab).Cast<AutomationElement>();

            foreach (var tab in tabs)
            {
                var tabGroup = new TabGroup(tab);
                var connecter = tab.FindAll(TreeScope.Children, cond).Cast<AutomationElement>().ToObservable().Publish();

                connecter.Where(tabItem => tabItem.IsSelected())
                    .Subscribe(tabItem => tabGroup.MarkInitial(tabItem));

                connecter.Subscribe(tabItem => tabGroup.Add(tabItem));

                connecter.Connect();

                tabGroups.Add(tabGroup);
            }
            return tabGroups;
        }
    }
}
