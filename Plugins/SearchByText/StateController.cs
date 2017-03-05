using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Enumerators;
using UIAssistant.Core.I18n;
using UIAssistant.Infrastructure.Logger;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Interfaces.Settings;
using UIAssistant.Plugin.SearchByText.Items;
using UIAssistant.Plugin.SearchByText.Enumerators;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.SearchByText
{
    internal class StateController : AbstractStateController
    {
        private ISearchByTextEnumerator _enumerator;
        private ICollection<IHUDItem> _sourceForFiltering, _contextSource;
        public bool AutoFire { get; set; }
        public IUserSettings Settings => SearchByText.UIAssistantAPI.UIAssistantSettings;

        public StateController(IUIAssistantAPI api) : base(api)
        {
        }

        public void Initialize()
        {
            SearchByText.UIAssistantAPI.DefaultHUD.Initialize();
            SearchByText.UIAssistantAPI.DefaultHUD.ItemsCountPerPage = Settings.ItemsCountPerPage;
            SearchByText.UIAssistantAPI.DefaultContextHUD.Initialize();
            SearchByText.UIAssistantAPI.DefaultContextHUD.ItemsCountPerPage = Settings.ItemsCountPerPage;
        }

        internal void Enumerate()
        {
            _sourceForFiltering = SearchByText.UIAssistantAPI.DefaultHUD.Items;
            _enumerator.Updated += (_, __) => Filter();
            _enumerator.Finished += (_, __) =>
            {
                _enumerator.Dispose();
                if (_sourceForFiltering.Count == 0)
                {
                    SearchByText.UIAssistantAPI.NotifyInfoMessage(Consts.PluginName, TextID.NoOneFound.GetLocalizedText());
                    Quit();
                }
                Filter();
            };
            Task.Run(() => _enumerator.Enumerate(_sourceForFiltering));
        }

        protected override void OnSwitchingToContext(bool isItemSelected)
        {
            SearchByText.UIAssistantAPI.DefaultContextHUD.Items.Clear();
            if (isItemSelected)
            {
                var selectedItem = SearchByText.UIAssistantAPI.DefaultHUD.SelectedItem;
                SearchByText.UIAssistantAPI.DefaultContextHUD.Items.Add(new Copy());
                if (selectedItem is IWindowItem)
                {
                    SearchByText.UIAssistantAPI.DefaultContextHUD.Items.Add(new CopyHwnd());
                    SearchByText.UIAssistantAPI.DefaultContextHUD.Items.Add(new ToggleTopMost());
                    SearchByText.UIAssistantAPI.DefaultContextHUD.Items.Add(new CloseWindow());
                }
            }
            SearchByText.UIAssistantAPI.DefaultContextHUD.Items.Add(new CopyAll());
            _contextSource = SearchByText.UIAssistantAPI.DefaultContextHUD.Items;
            SearchByText.UIAssistantAPI.DefaultContextHUD.TextBox.Clear();
        }

        internal void ChangeTarget(EnumerateTarget target)
        {
            _enumerator = Enumerator.Factory(target);
        }

        internal void Cancel()
        {
            _enumerator?.Cancel();
        }

        internal void Filter()
        {
            try
            {
                if (SearchByText.UIAssistantAPI.IsContextVisible)
                {
                    SearchByText.UIAssistantAPI.CurrentHUD.Filter(_contextSource, SearchByText.UIAssistantAPI.CurrentHUD.TextBox.Text);
                }
                else
                {
                    SearchByText.UIAssistantAPI.CurrentHUD.Filter(_sourceForFiltering, SearchByText.UIAssistantAPI.CurrentHUD.TextBox.Text);
                }
                if (AutoFire && SearchByText.UIAssistantAPI.DefaultHUD.Items.Count == 1)
                {
                    System.Threading.Thread.Sleep(300);
                    Execute();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        internal void Execute()
        {
            if (!(_enumerator is SearchForText) && !SearchByText.UIAssistantAPI.IsContextVisible)
            {
                var selectedItem = SearchByText.UIAssistantAPI.DefaultHUD.SelectedItem as SearchByTextItem;
                if (selectedItem == null || !selectedItem.IsEnabled)
                {
                    return;
                }
            }
            Cancel();
            SearchByText.UIAssistantAPI.CurrentHUD.Execute();
            Quit();
        }

        internal void Input(string input)
        {
            SearchByText.UIAssistantAPI.CurrentHUD.TextBox.Input(input);
        }

        public override void SwitchNextTheme()
        {
            SearchByText.UIAssistantAPI.NextTheme();
            SearchByText.UIAssistantAPI.UIAssistantSettings.Theme = SearchByText.UIAssistantAPI.CurrentTheme.Id;
            SearchByText.UIAssistantAPI.NotifyInfoMessage("Switch Theme", string.Format(TextID.SwitchTheme.GetLocalizedText(), SearchByText.UIAssistantAPI.UIAssistantSettings.Theme));
            SearchByText.UIAssistantAPI.UIAssistantSettings.Save();
        }

        public override void Quit()
        {
            Task.Run(() =>
            {
                Cleanup();
                SearchByText.UIAssistantAPI.RemoveDefaultHUD();
                SearchByText.UIAssistantAPI.RemoveContextHUD();
                SearchByText.UIAssistantAPI.TopMost = false;
            });
        }

        // TODO: Experimental
        internal void Expand()
        {
            Cancel();
            var selectedItem = SearchByText.UIAssistantAPI.DefaultHUD.SelectedItem as SearchByTextItem;
            if (selectedItem == null || !selectedItem.IsEnabled || !selectedItem.CanExpand)
            {
                return;
            }

            var activeWindow = Utility.Win32.Win32Window.ActiveWindow;

            // show popup
            selectedItem.Prepare();
            if (selectedItem is UIRibbonItem)
            {
                selectedItem.Execute();
                System.Threading.Thread.Sleep(300);
            }
            else
            {
                var element = selectedItem.GetCurrentElement(activeWindow.Element);
                element.SetFocus();
                element.TryExpand();
            }
            var popup = activeWindow.LastActivePopup;

            SearchByText.UIAssistantAPI.DefaultHUD.Initialize();
            _sourceForFiltering.Clear();

            SearchByText.UIAssistantAPI.TopMost = true;

            // Win32
            if (popup.WindowHandle == activeWindow.WindowHandle)
            {
                new SearchContainer().Enumerate(_sourceForFiltering);
                SearchByText.UIAssistantAPI.DefaultHUD.Items = _sourceForFiltering;
                return;
            }

            // WPF etc.
            var elements = popup.Element.FindAll(System.Windows.Automation.TreeScope.Descendants | System.Windows.Automation.TreeScope.Element, System.Windows.Automation.Condition.TrueCondition).Cast<System.Windows.Automation.AutomationElement>().ToList();
            elements.ForEach(x =>
            {
                var info = x.Current;
                var internalName = info.Name;

                if (!string.IsNullOrEmpty(internalName))
                {
                    var displayName = internalName.Trim();
                    if (_enumerator is SearchForText)
                    {
                        var item = new ItemInContainer(displayName, info.BoundingRectangle, info.IsEnabled, popup.Element, popup.Element);
                        _sourceForFiltering.Add(item);
                    }
                    else
                    {
                        var item = new UIRibbonItem(internalName, displayName, info.BoundingRectangle, info.IsEnabled, false, null, popup.Element);
                        _sourceForFiltering.Add(item);
                    }
                }
            });
            SearchByText.UIAssistantAPI.DefaultHUD.Items = _sourceForFiltering;
        }
    }
}
