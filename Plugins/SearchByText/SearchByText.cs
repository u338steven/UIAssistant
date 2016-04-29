using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;
using UIAssistant.Core.Enumerators;
using UIAssistant.Core.Commands;
using UIAssistant.Core.I18n;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.SearchByText
{
    [Export(typeof(IPlugin))]
    [Export(typeof(IConfigurablePlugin))]
    [Export(typeof(ILocalizablePlugin))]
    [Export(typeof(IDisposable))]
    [ExportMetadata("Name", "Search by Text")]
    [ExportMetadata("Author", "u338.steven")]
    [ExportMetadata("SupportUri", "https://github.com/u338steven/UIAssistant/")]
    [ExportMetadata("IconUri", "/SearchByText;component/Resources/SearchByText.png")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("CommandName", Consts.Command)]
    public class SearchByText : IPlugin, IConfigurablePlugin, ILocalizablePlugin, IDisposable
    {
        private StateController _stateController;
        private KeyInputController _keyController;

        public void Initialize()
        {
            _stateController = new StateController();
            _keyController = new KeyInputController(_stateController);
            RegisterCommand();
        }

        private void RegisterCommand()
        {
            var argCommands = new ArgumentNode(Consts.Commands);
            var argTextsInWindow = new ArgumentNode(Consts.TextsInWindow);
            var argTextsInContainer = new ArgumentNode(Consts.TextsInContainer);
            var argRunningApps = new ArgumentNode(Consts.RunningApps);

            var command = new CommandNode(Consts.Command, new[] { argCommands, argTextsInWindow, argTextsInContainer, argRunningApps, });

            UIAssistantAPI.RegisterCommand(command);
        }

        private Action Generate(EnumerateTarget target, IList<string> args)
        {
            return () =>
            {
                _stateController.Initialize();
                _keyController.Initialize();
                UIAssistantAPI.SwitchTheme(UIAssistantAPI.UIAssistantSettings.Theme);
                _stateController.ChangeTarget(target);
                _stateController.Enumerate();
                UIAssistantAPI.AddDefaultHUD();
                UIAssistantAPI.TopMost = true;
            };
        }

        public Action GenerateAction(IList<string> args)
        {
            if (args.Any(arg => Consts.Commands.EqualsWithCaseIgnored(arg)))
            {
                return Generate(EnumerateTarget.Commands, args);
            }
            else if (args.Any(arg => Consts.TextsInWindow.EqualsWithCaseIgnored(arg)))
            {
                return Generate(EnumerateTarget.TextsInWindow, args);
            }
            else if (args.Any(arg => Consts.TextsInContainer.EqualsWithCaseIgnored(arg)))
            {
                return Generate(EnumerateTarget.TextsInContainer, args);
            }
            else if (args.Any(arg => Consts.RunningApps.EqualsWithCaseIgnored(arg)))
            {
                return Generate(EnumerateTarget.RunningApps, args);
            }
            return Generate(EnumerateTarget.TextsInWindow, args);
        }

        public System.Windows.FrameworkElement GetConfigrationInterface()
        {
            return new Settings();
        }

        public void Save()
        {
            SearchByTextSettings.Instance.Save();
            _keyController.Reset();
        }

        static Localizer _localizer = new Localizer();
        public void Localize()
        {
            _localizer.SwitchLanguage(DefaultLocalizer.CurrentLanguage);

            SearchByTextSettings.Instance.Expand.Text = _localizer.GetLocalizedText("sbtExpand");
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _keyController?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
