using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;
using UIAssistant.Core.Enumerators;
using UIAssistant.Core.I18n;
using UIAssistant.Infrastructure.Commands;
using UIAssistant.Interfaces.Commands;
using UIAssistant.Interfaces.Plugin;

namespace UIAssistant.Plugin.SearchByText
{
    [Export(typeof(IPlugin))]
    [Export(typeof(IConfigurablePlugin))]
    [Export(typeof(ILocalizablePlugin))]
    [Export(typeof(IDisposable))]
    [ExportMetadata("Guid", "cd70037d-4b69-4c5d-8882-5db95fdd5127")]
    [ExportMetadata("Name", Consts.PluginName)]
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
            var argCommands = new ArgumentRule(Consts.Commands, _ => _stateController.ChangeTarget(EnumerateTarget.Commands));
            var argTextsInWindow = new ArgumentRule(Consts.TextsInWindow, _ => _stateController.ChangeTarget(EnumerateTarget.TextsInWindow));
            var argTextsInContainer = new ArgumentRule(Consts.TextsInContainer, _ => _stateController.ChangeTarget(EnumerateTarget.TextsInContainer));
            var argRunningApps = new ArgumentRule(Consts.RunningApps, _ => _stateController.ChangeTarget(EnumerateTarget.RunningApps));
            var argContextMenu = new ArgumentRule(Consts.ContextMenu, _ => _stateController.ChangeTarget(EnumerateTarget.ContextMenu));

            var optAutoFire = new ArgumentRule(Consts.AutoFire, _ => _stateController.AutoFire = true);

            var command = new CommandRule(Consts.Command, Run,
                new[] { argCommands, argTextsInWindow, argTextsInContainer, argRunningApps, argContextMenu },
                new[] { optAutoFire });
            command.Description = Consts.PluginName;
            UIAssistantAPI.RegisterCommand(command);
        }

        public void Setup()
        {
            _stateController.Initialize();
            _keyController.Initialize();
            UIAssistantAPI.SwitchTheme(UIAssistantAPI.UIAssistantSettings.Theme);
        }

        public void Run(ICommand command)
        {
            try
            {
                _stateController.Enumerate();
                UIAssistantAPI.AddDefaultHUD();
                UIAssistantAPI.AddContextHUD();
                UIAssistantAPI.TopMost = true;
            }
            catch (Exception ex)
            {
                UIAssistantAPI.PrintErrorMessage(ex);
            }
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
