using System;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel.Composition;

using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Commands;
using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Plugin.MouseEmulation
{
    [Export(typeof(IPlugin))]
    [Export(typeof(IConfigurablePlugin))]
    [Export(typeof(ILocalizablePlugin))]
    [Export(typeof(IDisposable))]
    [ExportMetadata("Guid", "b0744325-4b37-44f4-bb5d-683667f3f3ee")]
    [ExportMetadata("Name", Consts.PluginName)]
    [ExportMetadata("Author", "u338.steven")]
    [ExportMetadata("SupportUri", "https://github.com/u338steven/UIAssistant/")]
    [ExportMetadata("IconUri", "/MouseEmulation;component/Resources/MouseEmulation.png")]
    [ExportMetadata("Version", "1.0")]
    [ExportMetadata("CommandName", Consts.Command)]
    public class MouseEmulation : IPlugin, IConfigurablePlugin, ILocalizablePlugin, IDisposable
    {
        internal static MouseEmulationSettings Settings { get; private set; }
        internal static IUIAssistantAPI UIAssistantAPI { get; private set; }
        private static ILocalizer _localizer;

        public void Initialize(IUIAssistantAPI api)
        {
            UIAssistantAPI = api;

            Settings = MouseEmulationSettings.Load();
            _localizer = api.LocalizationAPI.GetLocalizer();
            RegisterCommand();
        }

        private void RegisterCommand()
        {
            var command = UIAssistantAPI.CommandAPI.CreateCommandRule(Consts.Command, Run);
            command.Description = Consts.PluginName;
            UIAssistantAPI.CommandAPI.RegisterCommand(command);
        }

        public void Setup()
        {

        }

        public void Run(ICommand command)
        {
            try
            {
                MouseController.Start();
            }
            catch (Exception ex)
            {
                UIAssistantAPI.LogAPI.WriteErrorMessage(ex);
            }
        }

        public FrameworkElement GetConfigrationInterface()
        {
            return new Settings();
        }

        public void Localize()
        {
            _localizer.SwitchLanguage(UIAssistantAPI.LocalizationAPI.CurrentLanguage);
            var settings = MouseEmulation.Settings;

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
            MouseEmulation.Settings.Save();
        }

        public void Dispose()
        {
            MouseController.Stop();
        }
    }

    class Consts
    {
        internal const string PluginName = "Mouse Emulation";
        internal const string Command = "me";
    }
}
