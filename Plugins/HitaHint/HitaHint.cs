using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;

using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Commands;
using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Resource;

using UIAssistant.Plugin.HitaHint.Enumerators;
using UIAssistant.Plugin.HitaHint.Operations;

namespace UIAssistant.Plugin.HitaHint
{
    [Export(typeof(IPlugin))]
    [Export(typeof(IConfigurablePlugin))]
    [Export(typeof(ILocalizablePlugin))]
    [Export(typeof(IDisposable))]
    [ExportMetadata("Guid", "dd89b0bf-b416-4329-8f9a-31051e810740")]
    [ExportMetadata("Name", Consts.PluginName)]
    [ExportMetadata("Author", "u338.steven")]
    [ExportMetadata("SupportUri", "https://github.com/u338steven/UIAssistant/")]
    [ExportMetadata("IconUri", "/HitaHint;component/Resources/HitaHint.png")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("CommandName", Consts.Command)]
    public class HitaHint : IPlugin, IConfigurablePlugin, ILocalizablePlugin, IDisposable
    {
        private StateController _stateController;
        private KeyInputController _keyController;
        internal static IUIAssistantAPI UIAssistantAPI { get; private set; }

        public void Initialize(IUIAssistantAPI api)
        {
            UIAssistantAPI = api;
            _stateController = new StateController(api);
            _keyController = new KeyInputController(api, _stateController);
            _localizer = api.GetLocalizer();
            RegisterCommand();
        }

        private void RegisterCommand()
        {
            var argSwitch = UIAssistantAPI.CreateArgmentRule(Consts.Switch, x => OperationManager.Change(x.Name));
            var argClick = UIAssistantAPI.CreateArgmentRule(Consts.Click, x => OperationManager.Change(x.Name));
            var argDoubleClick = UIAssistantAPI.CreateArgmentRule(Consts.DoubleClick, x => OperationManager.Change(x.Name));
            var argDragAndDrop = UIAssistantAPI.CreateArgmentRule(Consts.DragAndDrop, x => OperationManager.Change(x.Name));
            var argHover = UIAssistantAPI.CreateArgmentRule(Consts.Hover, x => OperationManager.Change(x.Name));
            var argMiddleClick = UIAssistantAPI.CreateArgmentRule(Consts.MiddleClick, x => OperationManager.Change(x.Name));
            var argMouseEmulation = UIAssistantAPI.CreateArgmentRule(Consts.MouseEmulation, x => OperationManager.Change(x.Name));
            var argRightClick = UIAssistantAPI.CreateArgmentRule(Consts.RightClick, x => OperationManager.Change(x.Name));

            var argRunningApps = UIAssistantAPI.CreateArgmentRule(Consts.RunningApps, x => _stateController.ChangeTarget(EnumerateTarget.RunningApps), new[] { argSwitch });
            var argWidgetsInWindow = UIAssistantAPI.CreateArgmentRule(Consts.WidgetsInWindow, x => _stateController.ChangeTarget(EnumerateTarget.WidgetsInWindow),
                new[] { argClick, argDoubleClick, argDragAndDrop, argHover, argMiddleClick, argMouseEmulation, argRightClick });
            var argWidgetsInTaskbar = UIAssistantAPI.CreateArgmentRule(Consts.WidgetsInTaskbar, x => _stateController.ChangeTarget(EnumerateTarget.WidgetsInTaskbar),
                new[] { argClick, argDoubleClick, argDragAndDrop, argHover, argMiddleClick, argMouseEmulation, argRightClick });
            var argDividedscreen = UIAssistantAPI.CreateArgmentRule(Consts.DividedScreen, x => _stateController.ChangeTarget(EnumerateTarget.DividedScreen),
                new[] { argClick, argDoubleClick, argDragAndDrop, argHover, argMiddleClick, argMouseEmulation, argRightClick });

            var optTheme = UIAssistantAPI.CreateArgmentRule(Consts.Theme, x => _stateController.SetTheme(x.Value));
            var optNoReturnCursor = UIAssistantAPI.CreateArgmentRule(Consts.NoReturnCursor, _ => _stateController.NoReturnCursor = true);

            var command = UIAssistantAPI.CreateCommandRule(Consts.Command, Run,
                new[] { argWidgetsInWindow, argRunningApps, argWidgetsInTaskbar, argDividedscreen, },
                new[] { optTheme, optNoReturnCursor });
            command.Description = Consts.PluginName;
            UIAssistantAPI.RegisterCommand(command);
        }

        public void Setup()
        {
            _keyController.Initialize();
            _stateController.Initialize();
        }

        private void Run(ICommand command)
        {
            try
            {
                _stateController.ApplyTheme();
                _stateController.PrintState();
                UIAssistantAPI.AddDefaultHUD();
                UIAssistantAPI.TopMost = true;
                _stateController.Enumerate();
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
            HitaHintSettings.Instance.Save();
            _keyController?.Reset();
            _stateController?.Reset();
        }

        static ILocalizer _localizer;// = new Localizer();
        public void Localize()
        {
            _localizer.SwitchLanguage(UIAssistantAPI.CurrentLanguage);

            HitaHintSettings.Instance.Click.Text = _localizer.GetLocalizedText("hahClick");
            HitaHintSettings.Instance.DoubleClick.Text = _localizer.GetLocalizedText("hahDoubleClick");
            HitaHintSettings.Instance.DragAndDrop.Text = _localizer.GetLocalizedText("hahDragAndDrop");
            HitaHintSettings.Instance.Hover.Text = _localizer.GetLocalizedText("hahHover");
            HitaHintSettings.Instance.MiddleClick.Text = _localizer.GetLocalizedText("hahMiddleClick");
            HitaHintSettings.Instance.MouseEmulation.Text = _localizer.GetLocalizedText("hahMouseEmulation");
            HitaHintSettings.Instance.Reload.Text = _localizer.GetLocalizedText("hahReload");
            HitaHintSettings.Instance.Reverse.Text = _localizer.GetLocalizedText("hahReverse");
            HitaHintSettings.Instance.RightClick.Text = _localizer.GetLocalizedText("hahRightClick");
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _stateController?.Dispose();
                    _stateController = null;
                    _keyController?.Dispose();
                    _keyController = null;
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
