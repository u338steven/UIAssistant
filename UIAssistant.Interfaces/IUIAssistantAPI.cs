using System;
using System.Collections.Generic;
using Data = System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using KeybindHelper.LowLevel;
using UIAssistant.Interfaces.Commands;
using UIAssistant.Interfaces.Events;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Resource;
using UIAssistant.Interfaces.Settings;

namespace UIAssistant.Interfaces.API
{
    public interface IUIAssistantAPI
    {
        IHUD CurrentHUD { get; }
        IHUD DefaultContextHUD { get; }
        IHUD DefaultHUD { get; }
        bool IsContextAvailable { get; }
        bool IsContextVisible { get; }
        bool TopMost { set; }
        bool Transparent { get; set; }
        string ConfigurationDirectory { get; }
        IFileIO DefaultSettingsFileIO { get; }
        IUserSettings UIAssistantSettings { get; }
        Dispatcher UIDispatcher { get; }

        void AddContextHUD();
        void AddDefaultHUD();
        Control AddIndicator();
        void AddPanel(UIElement uielement, Visibility visibility = Visibility.Visible);
        void AddTargetingReticle();
#if DEBUG
        void DisplayKeystroke(LowLevelKeyEventArgs e);
#endif
        void FlashIndicatorAnimation(Rect size, bool waitable = true, double duration = 300, Action completed = null);
        IEnumerable<string> GenerateHints(string hintKeys, int quantity);
        void Initialize(Control defaultHUDPanel, Control defaultContextPanel);
        string Localize(string id);
        void MoveTargetingReticle(double x, double y);
        void NotifyErrorMessage(string title, string message);
        void NotifyInfoMessage(string title, string message);
        void NotifyWarnMessage(string title, string message);
        IEnumerable<ICommand> ParseStatement(string statement);
        ICandidatesGenerator GetCommandGenerator();
        Data.ValidationResult Validate(string statement);
        IPluginManager PluginManager { get; }
        void PrintDebugMessage(string message);
        void PrintErrorMessage(Exception ex, string message = null);
        void RegisterCommand(ICommandRule rule);
        void RemoveContextHUD();
        void RemoveDefaultHUD();
        void RemoveIndicator(Control indicator);
        void RemovePanel(UIElement uielement);
        void RemoveTargetingReticle();
        void ScaleIndicatorAnimation(Rect from, Rect to, bool waitable = true, double duration = 300, Action completed = null);
        void SwitchHUD();

        IEventObserver GetObserver(ObserberKinds kind);
        ILocalizer GetLocalizer();
        IResourceItem CurrentLanguage { get; }
        ICommandRule CreateCommandRule(string name, Action<ICommand> action, ICollection<IArgumentRule> requiredArgs = null, ICollection<IArgumentRule> optionalArgs = null);
        IArgumentRule CreateArgmentRule(string name, Action<ICommand> action, ICollection<IArgumentRule> requiredArgs = null, ICollection<IArgumentRule> optionalArgs = null);

        IWindow ActiveWindow { get; }

        ITaskbar Taskbar { get; }
        IWindow FindWindow(string className, string caption = null);
        void EnumerateWindows(Func<IWindow, bool> func);
        IScreen Screen { get; }
        void InvokePluginCommand(string command, Action quit = null, Action pausing = null, Action resumed = null);

        IKeyboardAPI KeyboardAPI { get; }
        IMouseAPI MouseAPI { get; }
        ISessionAPI SessionAPI { get; }
        IThemeAPI ThemeAPI { get; }
    }
}
