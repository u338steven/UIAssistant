using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Themes;
using UIAssistant.Core.HitaHint;
using UIAssistant.Core.Enumerators;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using UIAssistant.Core.Commands;
using UIAssistant.Utility.Win32;
using UIAssistant.Core.Settings;
using UIAssistant.UI.Controls;
using KeybindHelper.LowLevel;

namespace UIAssistant.Plugin
{
    public static class UIAssistantAPI
    {
        private static Window window { get; set; } = Application.Current.MainWindow;
        private static Control hudPanel { set; get; }

        public static void Initialize(Control defaultHUDPanel)
        {
            if (hudPanel != null)
            {
                // already initialized
                return;
            }
            hudPanel = defaultHUDPanel;
        }

        public static System.Windows.Threading.Dispatcher UIDispatcher => window.Dispatcher;

        public static void SwitchTheme(string name)
        {
            window.Dispatcher.Invoke(() =>
            {
                ThemeDefaultSwitcher.Switch(name);
            });
        }

        public static void NextTheme()
        {
            window.Dispatcher.Invoke(() =>
            {
                ThemeDefaultSwitcher.Next();
            });
        }

        public static Theme CurrentTheme
        {
            get
            {
                return ThemeDefaultSwitcher.CurrentTheme;
            }
        }

        public static IHUDItemEnumerator GetWidgetEnumerator()
        {
            return new WidgetEnumerator();
        }

        public static IEnumerable<string> GenerateHints(string hintKeys, int quantity)
        {
            if (hintKeys.Contains('|'))
            {
                return AlternateHintGenerator.Generate(hintKeys, quantity);
            }
            return HintGenerator.Generate(hintKeys, quantity);
        }

        public static IHUD DefaultHUD
        {
            get
            {
                return window.Dispatcher.Invoke(() =>
                {
                    return hudPanel.DataContext as IHUD;
                });
            }
        }

        public static void AddDefaultHUD()
        {
            AddPanel(hudPanel);
        }

        public static void AddPanel(UIElement uielement)
        {
            window.Dispatcher.Invoke(() =>
            {
                var panel = window.Content as Panel;
                if (!panel.Children.Contains(uielement))
                {
                    panel.Children.Add(uielement);
                }
                uielement.Visibility = Visibility.Visible;
            });
        }

        public static void RemoveDefaultHUD()
        {
            RemovePanel(hudPanel);
            DefaultHUD.Initialize();
        }

        public static void RemovePanel(UIElement uielement)
        {
            if (uielement == null)
            {
                return;
            }
            window.Dispatcher.Invoke(() =>
            {
                var panel = window.Content as Panel;
                if (panel.Children.Contains(uielement))
                {
                    panel.Children.Remove(uielement);
                }
                uielement.Visibility = Visibility.Collapsed;
            });
        }

        public static bool TopMost
        {
            set
            {
                window.Dispatcher.Invoke(() =>
                {
                    window.Topmost = value;
                    if (value)
                    {
                        // force topmost
                        IntPtr windowHandle = new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle;
                        Win32Window.SetWindowPos(windowHandle, Win32Window.HWND.TopMost, false);
                    }
                });
            }
        }

        private static bool _transparent = false;
        public static bool Transparent
        {
            get
            {
                return _transparent;
            }
            set
            {
                _transparent = value;
                window.Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (value)
                    {
                        window.Opacity = 0.1d;
                    }
                    else
                    {
                        window.Opacity = 1d;
                    }
                }));
            }
        }

        public static string Localize(string id)
        {
            return Core.I18n.DefaultLocalizer.GetLocalizedText(id);
        }

        public static void RegisterCommand(CommandNode command)
        {
            CommandManager.Add(command);
        }

        public static UserSettings UIAssistantSettings { get { return UserSettings.Instance; } }
#if DEBUG
        public static void DisplayKeystroke(System.Windows.Input.Key key, KeySet keysState)
        {
            UIDispatcher.BeginInvoke((Action)(() => KeyVisualizer.Notify(key, keysState)));
        }
#endif
        public static void NotifyWarnMessage(string title, string message)
        {
            Notification.NotifyMessage(title, message, NotificationIcon.Warning);
        }

        public static void NotifyInfoMessage(string title, string message)
        {
            Notification.NotifyMessage(title, message, NotificationIcon.Information);
        }

        public static void NotifyErrorMessage(string title, string message)
        {
            Notification.NotifyMessage(title, message, NotificationIcon.Error);
        }

        static Control _reticle;
        public static void AddTargetingReticle()
        {
            _reticle = window.FindResource("TargetingReticle") as Control;
            var pt = Core.Input.MouseOperation.GetMousePosition();
            window.Dispatcher.Invoke(() =>
            {
                Canvas.SetLeft(_reticle, pt.X);
                Canvas.SetTop(_reticle, pt.Y);
            });
            AddPanel(_reticle);
        }

        public static void RemoveTargetingReticle()
        {
            RemovePanel(_reticle);
            _reticle = null;
        }

        public static void MoveTargetingReticle(double x, double y)
        {
            if ((Math.Abs(x) < 1 && Math.Abs(y) < 1) || _reticle == null)
            {
                return;
            }
            var pt = Core.Input.MouseOperation.GetMousePosition();
            window.Dispatcher.Invoke(() =>
            {
                Canvas.SetLeft(_reticle, pt.X);
                Canvas.SetTop(_reticle, pt.Y);
            });
        }

        public static Control Indicator { get; private set; }
        public static void AddIndicator()
        {
            Indicator = window.FindResource("Indicator") as Control;
            AddPanel(Indicator);
        }

        public static void RemoveIndicator()
        {
            RemovePanel(Indicator);
            Indicator = null;
        }

        private static AnimationTimeline GenerateAnimation(FrameworkElement element, string propertyName, double from, double to, double duration)
        {
            var animation = new DoubleAnimation() { From = from, To = to, Duration = new Duration(TimeSpan.FromMilliseconds(duration)) };

            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyName));
            return animation;
        }

        private static void AnimateWaitably(Action<Storyboard> animation, Action completion, bool waitable = true)
        {
            var isCompleted = false;
            window.Dispatcher.BeginInvoke((Action)(() =>
            {
                var storyboard = new Storyboard();
                animation?.Invoke(storyboard);

                storyboard.Completed += (o, e) =>
                {
                    completion?.Invoke();
                    isCompleted = true;
                };
                storyboard.Begin();
            }));
            while (!isCompleted && waitable)
            {
                System.Threading.Thread.Sleep(100);
            }
        }

        public static void ScaleIndicatorAnimation(Rect from, Rect to, bool waitable = true, double duration = 300)
        {
            AnimateWaitably(storyboard =>
            {
                AddIndicator();
                storyboard.Children.Add(GenerateAnimation(Indicator, "Opacity", 1d, 1d, duration));
                storyboard.Children.Add(GenerateAnimation(Indicator, "(Canvas.Left)", from.Left, to.Left, duration));
                storyboard.Children.Add(GenerateAnimation(Indicator, "(Canvas.Top)", from.Top, to.Top, duration));
                storyboard.Children.Add(GenerateAnimation(Indicator, "Width", from.Width, to.Width, duration));
                storyboard.Children.Add(GenerateAnimation(Indicator, "Height", from.Height, to.Height, duration));
            }, () => RemoveIndicator(), waitable);
        }

        public static void FlashIndicatorAnimation(Rect size, bool waitable = true, double duration = 300)
        {
            AnimateWaitably(storyboard =>
            {
                AddIndicator();
                Canvas.SetLeft(Indicator, size.X);
                Canvas.SetTop(Indicator, size.Y);
                Indicator.Width = size.Width;
                Indicator.Height = size.Height;

                var animation = GenerateAnimation(Indicator, "Opacity", 0d, 1d, duration);
                animation.RepeatBehavior = new RepeatBehavior(3);
                animation.AutoReverse = true;
                storyboard.Children.Add(animation);
            }, () => RemoveIndicator(), waitable);
        }
    }
}
