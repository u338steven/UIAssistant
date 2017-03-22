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
            UIAssistantAPI.ViewAPI.DefaultHUD.Initialize();
            UIAssistantAPI.ViewAPI.DefaultHUD.ItemsCountPerPage = Settings.ItemsCountPerPage;
            UIAssistantAPI.ViewAPI.DefaultContextHUD.Initialize();
            UIAssistantAPI.ViewAPI.DefaultContextHUD.ItemsCountPerPage = Settings.ItemsCountPerPage;
        }

        internal void Enumerate()
        {
            _sourceForFiltering = UIAssistantAPI.ViewAPI.DefaultHUD.Items;
            _enumerator.Updated += (_, __) => Filter();
            _enumerator.Finished += (_, __) =>
            {
                _enumerator.Dispose();
                if (_sourceForFiltering.Count == 0)
                {
                    UIAssistantAPI.NotificationAPI.NotifyInfoMessage(Consts.PluginName, UIAssistantAPI.Localize(TextID.NoOneFound));
                    Quit();
                }
                Filter();
            };
            Task.Run(() => _enumerator.Enumerate(_sourceForFiltering));
        }

        public void SwitchHUD()
        {
            if (!UIAssistantAPI.ViewAPI.IsContextAvailable)
            {
                return;
            }

            if (!UIAssistantAPI.ViewAPI.IsContextVisible)
            {
                OnSwitchingToContext(UIAssistantAPI.ViewAPI.DefaultHUD.SelectedItem != null);
            }

            UIAssistantAPI.ViewAPI.SwitchHUD();
        }

        private void OnSwitchingToContext(bool isItemSelected)
        {
            UIAssistantAPI.ViewAPI.DefaultContextHUD.Items.Clear();
            if (isItemSelected)
            {
                var selectedItem = UIAssistantAPI.ViewAPI.DefaultHUD.SelectedItem;
                UIAssistantAPI.ViewAPI.DefaultContextHUD.Items.Add(new Copy());
                if (selectedItem is IWindowItem)
                {
                    UIAssistantAPI.ViewAPI.DefaultContextHUD.Items.Add(new CopyHwnd());
                    UIAssistantAPI.ViewAPI.DefaultContextHUD.Items.Add(new ToggleTopMost());
                    UIAssistantAPI.ViewAPI.DefaultContextHUD.Items.Add(new CloseWindow());
                }
            }
            UIAssistantAPI.ViewAPI.DefaultContextHUD.Items.Add(new CopyAll());
            _contextSource = UIAssistantAPI.ViewAPI.DefaultContextHUD.Items;
            UIAssistantAPI.ViewAPI.DefaultContextHUD.TextBox.Clear();
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
                if (UIAssistantAPI.ViewAPI.IsContextVisible)
                {
                    UIAssistantAPI.ViewAPI.CurrentHUD.Filter(_contextSource, UIAssistantAPI.ViewAPI.CurrentHUD.TextBox.Text);
                }
                else
                {
                    UIAssistantAPI.ViewAPI.CurrentHUD.Filter(_sourceForFiltering, UIAssistantAPI.ViewAPI.CurrentHUD.TextBox.Text);
                }
                if (AutoFire && UIAssistantAPI.ViewAPI.DefaultHUD.Items.Count == 1)
                {
                    System.Threading.Thread.Sleep(300);
                    Execute();
                }
            }
            catch (Exception ex)
            {
                UIAssistantAPI.LogAPI.WriteErrorMessage(ex);
            }
        }

        internal void Execute()
        {
            if (!(_enumerator is SearchForText) && !UIAssistantAPI.ViewAPI.IsContextVisible)
            {
                var selectedItem = UIAssistantAPI.ViewAPI.DefaultHUD.SelectedItem as SearchByTextItem;
                if (selectedItem == null || !selectedItem.IsEnabled)
                {
                    return;
                }
            }
            Cancel();
            UIAssistantAPI.ViewAPI.CurrentHUD.Execute();
            Quit();
        }

        internal void Input(string input)
        {
            UIAssistantAPI.ViewAPI.CurrentHUD.TextBox.Input(input);
        }

        public void SwitchNextTheme()
        {
            UIAssistantAPI.ThemeAPI.NextTheme();
            UIAssistantAPI.UIAssistantSettings.Theme = UIAssistantAPI.ThemeAPI.CurrentTheme.Id;
            UIAssistantAPI.NotificationAPI.NotifyInfoMessage("Switch Theme", string.Format(UIAssistantAPI.Localize(TextID.SwitchTheme), UIAssistantAPI.UIAssistantSettings.Theme));
            UIAssistantAPI.UIAssistantSettings.Save();
        }

        public void Quit()
        {
            Task.Run(() =>
            {
                Session.Dispose();
                //Cleanup();
                UIAssistantAPI.ViewAPI.RemoveDefaultHUD();
                UIAssistantAPI.ViewAPI.RemoveContextHUD();
                UIAssistantAPI.ViewAPI.TopMost = false;
            });
        }

        // TODO: Experimental
        internal void Expand()
        {
            Cancel();
            var selectedItem = UIAssistantAPI.ViewAPI.DefaultHUD.SelectedItem as SearchByTextItem;
            if (selectedItem == null || !selectedItem.IsEnabled || !selectedItem.CanExpand)
            {
                return;
            }

            var activeWindow = UIAssistantAPI.WindowAPI.ActiveWindow;

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

            UIAssistantAPI.ViewAPI.DefaultHUD.Initialize();
            _sourceForFiltering.Clear();

            UIAssistantAPI.ViewAPI.TopMost = true;

            // Win32
            if (popup.WindowHandle == activeWindow.WindowHandle)
            {
                new SearchContainer().Enumerate(_sourceForFiltering);
                UIAssistantAPI.ViewAPI.DefaultHUD.Items = _sourceForFiltering;
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
            UIAssistantAPI.ViewAPI.DefaultHUD.Items = _sourceForFiltering;
        }
    }
}
