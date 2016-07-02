using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;

using UIAssistant.Core.Enumerators;
using UIAssistant.Core.I18n;
using UIAssistant.Core.Commands;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.HitaHint
{
    [Export(typeof(IPlugin))]
    [Export(typeof(IConfigurablePlugin))]
    [Export(typeof(ILocalizablePlugin))]
    [Export(typeof(IDisposable))]
    [ExportMetadata("Name", "Hit-a-Hint")]
    [ExportMetadata("Author", "u338.steven")]
    [ExportMetadata("SupportUri", "https://github.com/u338steven/UIAssistant/")]
    [ExportMetadata("IconUri", "/HitaHint;component/Resources/HitaHint.png")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("CommandName", Consts.Command)]
    public class HitaHint : IPlugin, IConfigurablePlugin, ILocalizablePlugin, IDisposable
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
            var argRunningApps = new ArgumentNode(Consts.RunningApps, Consts.Switch);
            var argWidgetsInWindow = new ArgumentNode(Consts.WidgetsInWindow, Consts.Click, Consts.DoubleClick, Consts.DragAndDrop, Consts.Hover, Consts.MiddleClick, Consts.MouseEmulation, Consts.RightClick);
            var argWidgetsInTaskbar = new ArgumentNode(Consts.WidgetsInTaskbar, Consts.Click, Consts.DoubleClick, Consts.DragAndDrop, Consts.Hover, Consts.MiddleClick, Consts.MouseEmulation, Consts.RightClick);
            var argDividedscreen = new ArgumentNode(Consts.DividedScreen, Consts.Click, Consts.DoubleClick, Consts.DragAndDrop, Consts.Hover, Consts.MiddleClick, Consts.MouseEmulation, Consts.RightClick);

            var command = new CommandNode(Consts.Command, new[] { argWidgetsInWindow, argRunningApps, argWidgetsInTaskbar, argDividedscreen, });

            UIAssistantAPI.RegisterCommand(command);
        }

        private Action Generate(EnumerateTarget target, IList<string> args)
        {
            return () =>
            {
                _keyController.Initialize();
                _stateController.Initialize();
                _stateController.ChangeTarget(target);
                _stateController.ParseArguments(args);
                _stateController.PrintState();
                UIAssistantAPI.AddDefaultHUD();
                UIAssistantAPI.TopMost = true;
                _stateController.Enumerate();
            };
        }

        public Action GenerateAction(IList<string> args)
        {
            if (args.Any(arg => Consts.WidgetsInWindow.EqualsWithCaseIgnored(arg)))
            {
                return Generate(EnumerateTarget.WidgetsInWindow, args);
            }
            else if (args.Any(arg => Consts.WidgetsInTaskbar.EqualsWithCaseIgnored(arg)))
            {
                return Generate(EnumerateTarget.WidgetsInTaskbar, args);
            }
            else if (args.Any(arg => Consts.RunningApps.EqualsWithCaseIgnored(arg)))
            {
                return Generate(EnumerateTarget.RunningApps, args);
            }
            else if (args.Any(arg => Consts.DividedScreen.EqualsWithCaseIgnored(arg)))
            {
                return Generate(EnumerateTarget.DividedScreen, args);
            }
            return Generate(EnumerateTarget.WidgetsInWindow, args);
        }

        public System.Windows.FrameworkElement GetConfigrationInterface()
        {
            return new Settings();
        }

        public void Save()
        {
            HitaHintSettings.Instance.Save();
            _keyController?.Reset();
        }

        static Localizer _localizer = new Localizer();
        public void Localize()
        {
            _localizer.SwitchLanguage(DefaultLocalizer.CurrentLanguage);

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
