﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Settings;
using UIAssistant.Core.I18n;
using UIAssistant.Core.Enumerators;
using UIAssistant.Plugin.SearchByText.Items;
using UIAssistant.Plugin.SearchByText.Enumerators;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.SearchByText
{
    internal class StateController : AbstractStateController
    {
        private ISearchByTextEnumerator _enumerator;
        private HUDItemCollection _sourceForFiltering, _contextSource;
        private bool _autoFire = false;
        public UserSettings Settings => UIAssistantAPI.UIAssistantSettings;

        public StateController()
        {
        }

        public void Initialize()
        {
            UIAssistantAPI.DefaultHUD.Initialize();
            UIAssistantAPI.DefaultHUD.ItemsCountPerPage = Settings.ItemsCountPerPage;
            UIAssistantAPI.DefaultContextHUD.Initialize();
            UIAssistantAPI.DefaultContextHUD.ItemsCountPerPage = Settings.ItemsCountPerPage;
        }

        internal void ParseArguments(IList<string> args)
        {
            _autoFire = false;
            if (args.Contains(Consts.AutoFire, StringComparer.CurrentCultureIgnoreCase))
            {
                _autoFire = true;
            }
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
                    UIAssistantAPI.NotifyInfoMessage(Consts.PluginName, TextID.NoOneFound.GetLocalizedText());
                    Quit();
                }
                Filter();
            };
            Task.Run(() => _enumerator.Enumerate(_sourceForFiltering));
        }

        // to abstract
        protected override void OnSwitchingToContext()
        {
            // prepare context items
            UIAssistantAPI.DefaultContextHUD.Items.Clear();
            var selectedItem = UIAssistantAPI.DefaultHUD.SelectedItem;
            UIAssistantAPI.DefaultContextHUD.Items.Add(new Copy());
            if (selectedItem is RunningApp)
            {
                UIAssistantAPI.DefaultContextHUD.Items.Add(new CopyHwnd());
                UIAssistantAPI.DefaultContextHUD.Items.Add(new ToggleTopMost());
                UIAssistantAPI.DefaultContextHUD.Items.Add(new CloseWindow());
            }
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
            if (UIAssistantAPI.IsContextVisible)
            {
                UIAssistantAPI.CurrentHUD.Filter(_contextSource, UIAssistantAPI.CurrentHUD.TextBox.Text);
            }
            else
            {
                UIAssistantAPI.CurrentHUD.Filter(_sourceForFiltering, UIAssistantAPI.CurrentHUD.TextBox.Text);
            }
            if (_autoFire && UIAssistantAPI.DefaultHUD.Items.Count == 1)
            {
                System.Threading.Thread.Sleep(300);
                Execute();
            }
        }

        internal void Execute()
        {
            if (!(_enumerator is SearchForText))
            {
                var selectedItem = UIAssistantAPI.DefaultHUD.SelectedItem as SearchByTextItem;
                if (selectedItem == null || !selectedItem.IsEnabled)
                {
                    return;
                }
            }
            Cancel();
            if (UIAssistantAPI.IsContextVisible)
            {
                (UIAssistantAPI.DefaultContextHUD.SelectedItem as ContextItemBase).Execute(UIAssistantAPI.DefaultHUD.SelectedItem);
            }
            else
            {
                UIAssistantAPI.DefaultHUD.Execute();
            }
            Quit();
        }

        internal void Input(string input)
        {
            UIAssistantAPI.CurrentHUD.TextBox.Input(input);
            Filter();
        }

        public override void SwitchNextTheme()
        {
            UIAssistantAPI.NextTheme();
            UIAssistantAPI.UIAssistantSettings.Theme = UIAssistantAPI.CurrentTheme.FileName;
            UIAssistantAPI.NotifyInfoMessage("Switch Theme", string.Format(TextID.SwitchTheme.GetLocalizedText(), UIAssistantAPI.UIAssistantSettings.Theme));
            UIAssistantAPI.UIAssistantSettings.Save();
        }

        public override void Quit()
        {
            Task.Run(() =>
            {
                Cleanup();
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
            UIAssistantAPI.DefaultHUD.Items = _sourceForFiltering;
        }
    }
}
