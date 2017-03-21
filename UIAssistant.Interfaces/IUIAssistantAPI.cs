using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using KeybindHelper.LowLevel;
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
        IPluginManager PluginManager { get; }
        void PrintDebugMessage(string message);
        void PrintErrorMessage(Exception ex, string message = null);
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

        IScreen Screen { get; }

        ICommandAPI CommandAPI { get; }
        IKeyboardAPI KeyboardAPI { get; }
        IMouseAPI MouseAPI { get; }
        ISessionAPI SessionAPI { get; }
        IThemeAPI ThemeAPI { get; }
        IWindowAPI WindowAPI { get; }
    }
}
