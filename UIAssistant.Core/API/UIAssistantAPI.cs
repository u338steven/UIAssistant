using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using KeybindHelper.LowLevel;
using UIAssistant.Core.HitaHint;
using UIAssistant.Core.I18n;
using UIAssistant.Core.Settings;
using UIAssistant.Infrastructure.Events;
using UIAssistant.Infrastructure.Logger;
using UIAssistant.Infrastructure.Settings;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Events;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Resource;
using UIAssistant.Interfaces.Settings;
using UIAssistant.UI.Controls;
using UIAssistant.Utility.Win32;

namespace UIAssistant.Core.API
{
    public class UIAssistantAPI : IUIAssistantAPI
    {
        public static IUIAssistantAPI Instance { get; } = new UIAssistantAPI();
        public IPluginManager PluginManager { get; private set; }

        private UIAssistantAPI()
        {
            DefaultSettingsFileIO = new YamlFileIO((path, ex) => NotificationAPI.NotifyWarnMessage("Load Settings Error", string.Format(Localize(TextID.SettingsLoadError), path)));
            UIAssistantSettings = DefaultSettingsFileIO.Read(typeof(UserSettings), UserSettings.FilePath) as UserSettings;
        }

        private Window window { get; set; } = Application.Current.MainWindow;
        private Control hudPanel { get; set; }
        private Control contextPanel { get; set; }
        private Control currentPanel { get; set; }
        public string ConfigurationDirectory { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configurations");
        public IFileIO DefaultSettingsFileIO { get; private set; }
        public IUserSettings UIAssistantSettings { get; private set; }

        public void Initialize(Control defaultHUDPanel, Control defaultContextPanel)
        {
            if (hudPanel != null)
            {
                // already initialized
                return;
            }
            hudPanel = defaultHUDPanel;
            contextPanel = defaultContextPanel;
            currentPanel = hudPanel;
            PluginManager = Plugin.PluginManager.Instance;
        }

        public System.Windows.Threading.Dispatcher UIDispatcher => window.Dispatcher;

        public IEnumerable<string> GenerateHints(string hintKeys, int quantity)
        {
            if (hintKeys.Contains('|'))
            {
                return AlternateHintGenerator.Generate(hintKeys, quantity);
            }
            return HintGenerator.Generate(hintKeys, quantity);
        }

        public bool IsContextAvailable { get; private set; }

        public bool IsContextVisible
        {
            get { return (currentPanel == contextPanel); }
        }

        public IHUD CurrentHUD
        {
            get
            {
                return window.Dispatcher.Invoke(() =>
                {
                    return currentPanel.DataContext as IHUD;
                });
            }
        }

        public IHUD DefaultHUD
        {
            get
            {
                return window.Dispatcher.Invoke(() =>
                {
                    return hudPanel.DataContext as IHUD;
                });
            }
        }

        public void AddDefaultHUD()
        {
            AddPanel(hudPanel);
        }

        public IHUD DefaultContextHUD
        {
            get
            {
                return window.Dispatcher.Invoke(() =>
                {
                    return contextPanel.DataContext as IHUD;
                });
            }
        }

        public void AddContextHUD()
        {
            AddPanel(contextPanel, Visibility.Hidden);
            IsContextAvailable = true;
        }

        public void AddPanel(UIElement uielement, Visibility visibility = Visibility.Visible)
        {
            window.Dispatcher.Invoke(() =>
            {
                var panel = window.Content as Panel;
                if (!panel.Children.Contains(uielement))
                {
                    panel.Children.Add(uielement);
                }
                uielement.Visibility = visibility;
            });
        }

        public void SwitchHUD()
        {
            window.Dispatcher.Invoke(() =>
            {
                currentPanel.Visibility = Visibility.Hidden;
                if (currentPanel == hudPanel)
                {
                    currentPanel = contextPanel;
                }
                else
                {
                    currentPanel = hudPanel;
                }
                currentPanel.Visibility = Visibility.Visible;
            });
        }

        public void RemoveDefaultHUD()
        {
            RemovePanel(hudPanel);
            DefaultHUD.Initialize();
            currentPanel = hudPanel;
        }

        public void RemoveContextHUD()
        {
            RemovePanel(contextPanel);
            DefaultContextHUD.Initialize();
            IsContextAvailable = false;
        }

        public void RemovePanel(UIElement uielement)
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

        public bool TopMost
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

        private bool _transparent = false;
        public bool Transparent
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

        public string Localize(string id)
        {
            return I18n.DefaultLocalizer.GetLocalizedText(id);
        }

#if DEBUG
        public void DisplayKeystroke(LowLevelKeyEventArgs e)
        {
            KeyVisualizer.Notify(e);
        }
#endif
        public void PrintDebugMessage(string message)
        {
            Log.Debug(message);
        }

        public void PrintErrorMessage(Exception ex, string message = null)
        {
            Log.Error(ex, message);
        }

        Control _reticle;
        public void AddTargetingReticle()
        {
            _reticle = window.FindResource("TargetingReticle") as Control;
            var pt = MouseAPI.MouseOperation.GetMousePosition();
            window.Dispatcher.Invoke(() =>
            {
                Canvas.SetLeft(_reticle, pt.X);
                Canvas.SetTop(_reticle, pt.Y);
            });
            AddPanel(_reticle);
        }

        public void RemoveTargetingReticle()
        {
            RemovePanel(_reticle);
            _reticle = null;
        }

        public void MoveTargetingReticle(double x, double y)
        {
            if ((Math.Abs(x) < 1 && Math.Abs(y) < 1) || _reticle == null)
            {
                return;
            }
            var pt = MouseAPI.MouseOperation.GetMousePosition();
            window.Dispatcher.Invoke(() =>
            {
                Canvas.SetLeft(_reticle, pt.X);
                Canvas.SetTop(_reticle, pt.Y);
            });
        }

        public Control AddIndicator()
        {
            var Indicator = window.FindResource("Indicator") as Control;
            AddPanel(Indicator);
            return Indicator;
        }

        public void RemoveIndicator(Control indicator)
        {
            RemovePanel(indicator);
        }

        private AnimationTimeline GenerateAnimation(FrameworkElement element, string propertyName, double from, double to, double duration)
        {
            var animation = new DoubleAnimation() { From = from, To = to, Duration = new Duration(TimeSpan.FromMilliseconds(duration)) };

            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyName));
            return animation;
        }

        private void AnimateWaitably(Action<Storyboard> animation, Action completion, bool waitable = true)
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

        public void ScaleIndicatorAnimation(Rect from, Rect to, bool waitable = true, double duration = 300, Action completed = null)
        {
            Control indicator = null;
            AnimateWaitably(storyboard =>
            {
                indicator = AddIndicator();
                storyboard.Children.Add(GenerateAnimation(indicator, "Opacity", 1d, 1d, duration));
                storyboard.Children.Add(GenerateAnimation(indicator, "(Canvas.Left)", from.Left, to.Left, duration));
                storyboard.Children.Add(GenerateAnimation(indicator, "(Canvas.Top)", from.Top, to.Top, duration));
                storyboard.Children.Add(GenerateAnimation(indicator, "Width", from.Width, to.Width, duration));
                storyboard.Children.Add(GenerateAnimation(indicator, "Height", from.Height, to.Height, duration));
            }, () => { RemoveIndicator(indicator); completed?.Invoke(); }, waitable);
        }

        public void FlashIndicatorAnimation(Rect size, bool waitable = true, double duration = 300, Action completed = null)
        {
            Control indicator = null;
            AnimateWaitably(storyboard =>
            {
                indicator = AddIndicator();
                Canvas.SetLeft(indicator, size.X);
                Canvas.SetTop(indicator, size.Y);
                indicator.Width = size.Width;
                indicator.Height = size.Height;

                var animation = GenerateAnimation(indicator, "Opacity", 0d, 1d, duration);
                animation.RepeatBehavior = new RepeatBehavior(3);
                animation.AutoReverse = true;
                storyboard.Children.Add(animation);
            }, () => { RemoveIndicator(indicator); completed?.Invoke(); }, waitable);
        }

        public IEventObserver GetObserver(ObserberKinds kind)
        {
            switch (kind)
            {
                case ObserberKinds.StructureChangedObserver:
                    return new StructureChangedObserver();
                case ObserberKinds.FocusObserver:
                    return new FocusObserver();
                case ObserberKinds.PopupObserver:
                    return new PopupObserver();
                default:
                    return null;
            }
        }

        public ILocalizer GetLocalizer()
        {
            return new Localizer(Directory.GetParent(System.Reflection.Assembly.GetCallingAssembly().Location).ToString());
        }

        public IResourceItem CurrentLanguage { get { return DefaultLocalizer.CurrentLanguage; } }
        public IScreen Screen { get { return new Utility.Screen(); } }

        public ICommandAPI CommandAPI { get; } = new CommandAPI();
        public IKeyboardAPI KeyboardAPI { get; } = new KeyboardAPI();
        public IMouseAPI MouseAPI { get; } = new MouseAPI();
        public INotificationAPI NotificationAPI { get; } = new NotificationAPI();
        public ISessionAPI SessionAPI { get; } = new SessionAPI();
        public IThemeAPI ThemeAPI { get; } = new ThemeAPI();
        public IWindowAPI WindowAPI { get; } = new WindowAPI();
    }
}
