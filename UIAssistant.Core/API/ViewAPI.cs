using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

using KeybindHelper.LowLevel;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Utility.Win32;
using UIAssistant.UI.Controls;

namespace UIAssistant.Core.API
{
    class ViewAPI : IViewAPI
    {
        public bool IsContextAvailable { get; private set; }
        public Dispatcher UIDispatcher => _window.Dispatcher;

        private Control _contextPanel { get; set; }
        private Control _currentPanel { get; set; }
        private Control _hudPanel { get; set; }
        private Window _window { get; set; } = Application.Current.MainWindow;

        public ViewAPI(Control defaultHUDPanel, Control defaultContextPanel)
        {
            if (_hudPanel != null)
            {
                // already initialized
                return;
            }
            _hudPanel = defaultHUDPanel;
            _contextPanel = defaultContextPanel;
            _currentPanel = _hudPanel;
        }

        public bool IsContextVisible
        {
            get { return (_currentPanel == _contextPanel); }
        }

        public IHUD CurrentHUD
        {
            get
            {
                return _window.Dispatcher.Invoke(() =>
                {
                    return _currentPanel.DataContext as IHUD;
                });
            }
        }

        public IHUD DefaultHUD
        {
            get
            {
                return _window.Dispatcher.Invoke(() =>
                {
                    return _hudPanel.DataContext as IHUD;
                });
            }
        }

        public void AddDefaultHUD()
        {
            AddPanel(_hudPanel);
        }

        public IHUD DefaultContextHUD
        {
            get
            {
                return _window.Dispatcher.Invoke(() =>
                {
                    return _contextPanel.DataContext as IHUD;
                });
            }
        }

        public void AddContextHUD()
        {
            AddPanel(_contextPanel, Visibility.Hidden);
            IsContextAvailable = true;
        }

        public void AddPanel(UIElement uielement, Visibility visibility = Visibility.Visible)
        {
            _window.Dispatcher.Invoke(() =>
            {
                var panel = _window.Content as Panel;
                if (!panel.Children.Contains(uielement))
                {
                    panel.Children.Add(uielement);
                }
                uielement.Visibility = visibility;
            });
        }

        public void SwitchHUD()
        {
            _window.Dispatcher.Invoke(() =>
            {
                _currentPanel.Visibility = Visibility.Hidden;
                if (_currentPanel == _hudPanel)
                {
                    _currentPanel = _contextPanel;
                }
                else
                {
                    _currentPanel = _hudPanel;
                }
                _currentPanel.Visibility = Visibility.Visible;
            });
        }

        public void RemoveDefaultHUD()
        {
            RemovePanel(_hudPanel);
            DefaultHUD.Initialize();
            _currentPanel = _hudPanel;
        }

        public void RemoveContextHUD()
        {
            RemovePanel(_contextPanel);
            DefaultContextHUD.Initialize();
            IsContextAvailable = false;
        }

        public void RemovePanel(UIElement uielement)
        {
            if (uielement == null)
            {
                return;
            }
            _window.Dispatcher.Invoke(() =>
            {
                var panel = _window.Content as Panel;
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
                _window.Dispatcher.Invoke(() =>
                {
                    _window.Topmost = value;
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
                _window.Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (value)
                    {
                        _window.Opacity = 0.1d;
                    }
                    else
                    {
                        _window.Opacity = 1d;
                    }
                }));
            }
        }

        Control _reticle;
        public void AddTargetingReticle()
        {
            _reticle = _window.FindResource("TargetingReticle") as Control;
            var pt = UIAssistantAPI.Instance.MouseAPI.MouseOperation.GetMousePosition();
            _window.Dispatcher.Invoke(() =>
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
            var pt = UIAssistantAPI.Instance.MouseAPI.MouseOperation.GetMousePosition();
            _window.Dispatcher.Invoke(() =>
            {
                Canvas.SetLeft(_reticle, pt.X);
                Canvas.SetTop(_reticle, pt.Y);
            });
        }

        public Control AddIndicator()
        {
            var Indicator = _window.FindResource("Indicator") as Control;
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
            _window.Dispatcher.BeginInvoke((Action)(() =>
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

#if DEBUG
        public void DisplayKeystroke(LowLevelKeyEventArgs e)
        {
            KeyVisualizer.Notify(e);
        }
#endif
    }
}
