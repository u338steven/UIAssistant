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
        internal static IUIAssistantAPI UIAssistantAPI { get; private set; }
        internal static HitaHintSettings Settings { get; private set; }
        private static ILocalizer _localizer;
        private StateController _stateController;
        private KeyInputController _keyController;

        public void Initialize(IUIAssistantAPI api)
        {
            UIAssistantAPI = api;

            Settings = HitaHintSettings.Load();
            _stateController = new StateController(api);
            _keyController = new KeyInputController(api, _stateController);
            _localizer = api.GetLocalizer();
            RegisterCommand();
        }

        private void RegisterCommand()
        {
            var commandAPI = UIAssistantAPI.CommandAPI;
            var argSwitch = commandAPI.CreateArgmentRule(Consts.Switch, x => OperationManager.Change(x.Name));
            var argClick = commandAPI.CreateArgmentRule(Consts.Click, x => OperationManager.Change(x.Name));
            var argDoubleClick = commandAPI.CreateArgmentRule(Consts.DoubleClick, x => OperationManager.Change(x.Name));
            var argDragAndDrop = commandAPI.CreateArgmentRule(Consts.DragAndDrop, x => OperationManager.Change(x.Name));
            var argHover = commandAPI.CreateArgmentRule(Consts.Hover, x => OperationManager.Change(x.Name));
            var argMiddleClick = commandAPI.CreateArgmentRule(Consts.MiddleClick, x => OperationManager.Change(x.Name));
            var argMouseEmulation = commandAPI.CreateArgmentRule(Consts.MouseEmulation, x => OperationManager.Change(x.Name));
            var argRightClick = commandAPI.CreateArgmentRule(Consts.RightClick, x => OperationManager.Change(x.Name));

            var argRunningApps = commandAPI.CreateArgmentRule(Consts.RunningApps, x => _stateController.ChangeTarget(EnumerateTarget.RunningApps), new[] { argSwitch });
            var argWidgetsInWindow = commandAPI.CreateArgmentRule(Consts.WidgetsInWindow, x => _stateController.ChangeTarget(EnumerateTarget.WidgetsInWindow),
                new[] { argClick, argDoubleClick, argDragAndDrop, argHover, argMiddleClick, argMouseEmulation, argRightClick });
            var argWidgetsInTaskbar = commandAPI.CreateArgmentRule(Consts.WidgetsInTaskbar, x => _stateController.ChangeTarget(EnumerateTarget.WidgetsInTaskbar),
                new[] { argClick, argDoubleClick, argDragAndDrop, argHover, argMiddleClick, argMouseEmulation, argRightClick });
            var argDividedscreen = commandAPI.CreateArgmentRule(Consts.DividedScreen, x => _stateController.ChangeTarget(EnumerateTarget.DividedScreen),
                new[] { argClick, argDoubleClick, argDragAndDrop, argHover, argMiddleClick, argMouseEmulation, argRightClick });

            var optTheme = commandAPI.CreateArgmentRule(Consts.Theme, x => _stateController.SetTemporaryTheme(x.Value));
            var optNoReturnCursor = commandAPI.CreateArgmentRule(Consts.NoReturnCursor, _ => _stateController.NoReturnCursor = true);

            var command = commandAPI.CreateCommandRule(Consts.Command, Run,
                new[] { argWidgetsInWindow, argRunningApps, argWidgetsInTaskbar, argDividedscreen, },
                new[] { optTheme, optNoReturnCursor });
            command.Description = Consts.PluginName;
            commandAPI.RegisterCommand(command);
        }

        public void Setup()
        {
            _stateController.Initialize();
            var keyController = UIAssistantAPI.KeyboardAPI.CreateKeyboardController(_keyController, _stateController.State.Session);
            keyController.AddHidingProcess();
            UIAssistantAPI.ViewAPI.UIDispatcher.Invoke(() => keyController.AddUsagePanelProcess(new Usage()));
            keyController.Observe();
        }

        private void Run(ICommand command)
        {
            try
            {
                _stateController.ApplyTheme();
                _stateController.PrintState();
                UIAssistantAPI.ViewAPI.AddDefaultHUD();
                UIAssistantAPI.ViewAPI.TopMost = true;
                _stateController.Enumerate();
            }
            catch (Exception ex)
            {
                UIAssistantAPI.LogAPI.WriteErrorMessage(ex);
            }
        }

        public System.Windows.FrameworkElement GetConfigrationInterface()
        {
            return new Settings();
        }

        public void Save()
        {
            Settings.Save();
        }

        public void Localize()
        {
            _localizer.SwitchLanguage(UIAssistantAPI.CurrentLanguage);

            Settings.Click.Text = _localizer.GetLocalizedText("hahClick");
            Settings.DoubleClick.Text = _localizer.GetLocalizedText("hahDoubleClick");
            Settings.DragAndDrop.Text = _localizer.GetLocalizedText("hahDragAndDrop");
            Settings.Hover.Text = _localizer.GetLocalizedText("hahHover");
            Settings.MiddleClick.Text = _localizer.GetLocalizedText("hahMiddleClick");
            Settings.MouseEmulation.Text = _localizer.GetLocalizedText("hahMouseEmulation");
            Settings.Reload.Text = _localizer.GetLocalizedText("hahReload");
            Settings.Reverse.Text = _localizer.GetLocalizedText("hahReverse");
            Settings.RightClick.Text = _localizer.GetLocalizedText("hahRightClick");
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
