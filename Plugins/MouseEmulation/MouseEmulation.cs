using System;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel.Composition;

using UIAssistant.Core.I18n;
using UIAssistant.Core.Commands;

namespace UIAssistant.Plugin.MouseEmulation
{
    [Export(typeof(IPlugin))]
    [Export(typeof(IConfigurablePlugin))]
    [Export(typeof(ILocalizablePlugin))]
    [Export(typeof(IDisposable))]
    [ExportMetadata("Guid", "b0744325-4b37-44f4-bb5d-683667f3f3ee")]
    [ExportMetadata("Name", "Mouse Emulation")]
    [ExportMetadata("Author", "u338.steven")]
    [ExportMetadata("SupportUri", "https://github.com/u338steven/UIAssistant/")]
    [ExportMetadata("IconUri", "/MouseEmulation;component/Resources/MouseEmulation.png")]
    [ExportMetadata("Version", "1.0")]
    [ExportMetadata("CommandName", Consts.Command)]
    public class MouseEmulation : IPlugin, IConfigurablePlugin, ILocalizablePlugin, IDisposable
    {
        public void Initialize()
        {
            RegisterCommand();
        }

        private void RegisterCommand()
        {
            var command = new CommandNode(Consts.Command);
            UIAssistantAPI.RegisterCommand(command);
        }

        public Action GenerateAction(IList<string> args)
        {
            return () =>
            {
                try
                {
                    MouseController.Start();
                }
                catch (Exception ex)
                {
                    UIAssistantAPI.PrintErrorMessage(ex);
                }
            };
        }

        public FrameworkElement GetConfigrationInterface()
        {
            return new Settings();
        }

        static Localizer _localizer = new Localizer();
        public void Localize()
        {
            _localizer.SwitchLanguage(DefaultLocalizer.CurrentLanguage);
            var settings = MouseEmulationSettings.Instance;

            settings.Click.Text = _localizer.GetLocalizedText("meClick");
            settings.MiddleClick.Text = _localizer.GetLocalizedText("meMiddleClick");
            settings.RightClick.Text = _localizer.GetLocalizedText("meRightClick");

            settings.Left.Text = _localizer.GetLocalizedText("meLeft");
            settings.Right.Text = _localizer.GetLocalizedText("meRight");
            settings.Up.Text = _localizer.GetLocalizedText("meUp");
            settings.Down.Text = _localizer.GetLocalizedText("meDown");

            settings.WheelUp.Text = _localizer.GetLocalizedText("meWheelUp");
            settings.WheelDown.Text = _localizer.GetLocalizedText("meWheelDown");
            settings.HWheelUp.Text = _localizer.GetLocalizedText("meHWheelUp");
            settings.HWheelDown.Text = _localizer.GetLocalizedText("meHWheelDown");
            settings.SpeedUp.Text = _localizer.GetLocalizedText("meSpeedUp");
            settings.SlowDown.Text = _localizer.GetLocalizedText("meSlowDown");
        }

        public void Save()
        {
            MouseEmulationSettings.Instance.Save();
        }

        public void Dispose()
        {
            MouseController.Stop();
        }
    }

    class Consts
    {
        internal const string Command = "me";
    }
}
