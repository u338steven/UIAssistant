using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Automation;

using UIAssistant.Interfaces;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Interfaces.Session;
using UIAssistant.Interfaces.Settings;
using UIAssistant.Plugin.SearchByText.Items;
using UIAssistant.Plugin.SearchByText.Enumerators;

namespace UIAssistant.Plugin.SearchByText
{
    internal class StateController
    {
        private ISearchByTextEnumerator _enumerator;
        private ICollection<IHUDItem> _sourceForFiltering, _contextSource;
        public bool AutoFire { get; set; }
        public IUserSettings Settings => UIAssistantAPI.UIAssistantSettings;
        public ISession Session { get; private set; }
        IUIAssistantAPI UIAssistantAPI;

        public StateController(IUIAssistantAPI api)
        {
            UIAssistantAPI = api;
        }

        public void Initialize()
        {
            Session = UIAssistantAPI.SessionAPI.Create();
            UIAssistantAPI.DefaultHUD.Initialize();
            UIAssistantAPI.DefaultHUD.ItemsCountPerPage = Settings.ItemsCountPerPage;
            UIAssistantAPI.DefaultContextHUD.Initialize();
            UIAssistantAPI.DefaultContextHUD.ItemsCountPerPage = Settings.ItemsCountPerPage;
        }

        internal void Enumerate()
        {
            _sourceForFiltering = UIAssistantAPI.DefaultHUD.Items;
            _enumerator.Updated += (_, __) => Filter();
            _enumerator.Finished += (_, __) =>
            {
                _enumerator.Dispose();
                if (_sourceForFiltering.Count == 0)
                {
                    UIAssistantAPI.NotifyInfoMessage(Consts.PluginName, UIAssistantAPI.Localize(TextID.NoOneFound));
                    Quit();
                }
                Filter();
            };
            Task.Run(() => _enumerator.Enumerate(_sourceForFiltering));
        }

        public void SwitchHUD()
        {
            if (!UIAssistantAPI.IsContextAvailable)
            {
                return;
            }

            if (!UIAssistantAPI.IsContextVisible)
            {
                OnSwitchingToContext(UIAssistantAPI.DefaultHUD.SelectedItem != null);
            }

            UIAssistantAPI.SwitchHUD();
        }

        private void OnSwitchingToContext(bool isItemSelected)
        {
            UIAssistantAPI.DefaultContextHUD.Items.Clear();
            if (isItemSelected)
            {
                var selectedItem = UIAssistantAPI.DefaultHUD.SelectedItem;
                UIAssistantAPI.DefaultContextHUD.Items.Add(new Copy());
                if (selectedItem is IWindowItem)
                {
                    UIAssistantAPI.DefaultContextHUD.Items.Add(new CopyHwnd());
                    UIAssistantAPI.DefaultContextHUD.Items.Add(new ToggleTopMost());
                    UIAssistantAPI.DefaultContextHUD.Items.Add(new CloseWindow());
                }
            }
            UIAssistantAPI.DefaultContextHUD.Items.Add(new CopyAll());
            _contextSource = UIAssistantAPI.DefaultContextHUD.Items;
            UIAssistantAPI.DefaultContextHUD.TextBox.Clear();
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
                if (UIAssistantAPI.IsContextVisible)
                {
                    UIAssistantAPI.CurrentHUD.Filter(_contextSource, UIAssistantAPI.CurrentHUD.TextBox.Text);
                }
                else
                {
                    UIAssistantAPI.CurrentHUD.Filter(_sourceForFiltering, UIAssistantAPI.CurrentHUD.TextBox.Text);
                }
                if (AutoFire && UIAssistantAPI.DefaultHUD.Items.Count == 1)
                {
                    System.Threading.Thread.Sleep(300);
                    Execute();
                }
            }
            catch (Exception ex)
            {
                UIAssistantAPI.PrintErrorMessage(ex);
            }
        }

        internal void Execute()
        {
            if (!(_enumerator is SearchForText) && !UIAssistantAPI.IsContextVisible)
            {
                var selectedItem = UIAssistantAPI.DefaultHUD.SelectedItem as SearchByTextItem;
                if (selectedItem == null || !selectedItem.IsEnabled)
                {
                    return;
                }
            }
            Cancel();
            UIAssistantAPI.CurrentHUD.Execute();
            Quit();
        }

        internal void Input(string input)
        {
            UIAssistantAPI.CurrentHUD.TextBox.Input(input);
        }

        public void SwitchNextTheme()
        {
            UIAssistantAPI.ThemeAPI.NextTheme();
            UIAssistantAPI.UIAssistantSettings.Theme = UIAssistantAPI.ThemeAPI.CurrentTheme.Id;
            UIAssistantAPI.NotifyInfoMessage("Switch Theme", string.Format(UIAssistantAPI.Localize(TextID.SwitchTheme), UIAssistantAPI.UIAssistantSettings.Theme));
            UIAssistantAPI.UIAssistantSettings.Save();
        }

        public void Quit()
        {
            Task.Run(() =>
            {
                Session.Dispose();
                //Cleanup();
                UIAssistantAPI.RemoveDefaultHUD();
                UIAssistantAPI.RemoveContextHUD();
                UIAssistantAPI.TopMost = false;
            });
        }

        // TODO: Experimental
        internal void Expand()
        {
            Cancel();
            var selectedItem = UIAssistantAPI.DefaultHUD.SelectedItem as SearchByTextItem;
            if (selectedItem == null || !selectedItem.IsEnabled || !selectedItem.CanExpand)
            {
                return;
            }

            var activeWindow = UIAssistantAPI.ActiveWindow;

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

            UIAssistantAPI.DefaultHUD.Initialize();
            _sourceForFiltering.Clear();

            UIAssistantAPI.TopMost = true;

            // Win32
            if (popup.WindowHandle == activeWindow.WindowHandle)
            {
                new SearchContainer().Enumerate(_sourceForFiltering);
                UIAssistantAPI.DefaultHUD.Items = _sourceForFiltering;
                return;
            }

            // WPF etc.
            var elements = popup.Element.FindAll(TreeScope.Descendants | TreeScope.Element, Condition.TrueCondition).Cast<AutomationElement>().ToList();
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
            UIAssistantAPI.DefaultHUD.Items = _sourceForFiltering;
        }
    }
}
