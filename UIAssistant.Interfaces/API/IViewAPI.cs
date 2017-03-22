using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using KeybindHelper.LowLevel;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Interfaces.API
{
    public interface IViewAPI
    {
        IHUD CurrentHUD { get; }
        IHUD DefaultContextHUD { get; }
        IHUD DefaultHUD { get; }
        bool IsContextAvailable { get; }
        bool IsContextVisible { get; }
        bool TopMost { set; }
        bool Transparent { get; set; }
        Dispatcher UIDispatcher { get; }

        void AddContextHUD();
        void AddDefaultHUD();
        Control AddIndicator();
        void AddPanel(UIElement uielement, Visibility visibility = Visibility.Visible);
        void AddTargetingReticle();
        void FlashIndicatorAnimation(Rect size, bool waitable = true, double duration = 300, Action completed = null);
        void MoveTargetingReticle(double x, double y);
        void RemoveContextHUD();
        void RemoveDefaultHUD();
        void RemoveIndicator(Control indicator);
        void RemovePanel(UIElement uielement);
        void RemoveTargetingReticle();
        void ScaleIndicatorAnimation(Rect from, Rect to, bool waitable = true, double duration = 300, Action completed = null);
        void SwitchHUD();
#if DEBUG
        void DisplayKeystroke(LowLevelKeyEventArgs e);
#endif
    }
}