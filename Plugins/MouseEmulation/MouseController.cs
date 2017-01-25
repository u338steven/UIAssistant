using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Input;
using System.Timers;
using UIAssistant.Core.Input;
using UIAssistant.Core.Settings;
using KeybindHelper.LowLevel;

namespace UIAssistant.Plugin.MouseEmulation
{
    public static class MouseController
    {
        public static event Action Finished;
        static Timer _timer = new Timer();
        static bool _isPressedLbutton = false;
        static bool _isPressedRbutton = false;
        static bool _isPressedMbutton = false;
        static LowLevelKeyHook _keyHook;
        static UserSettings _userSettings;
        static MouseEmulationSettings _mouseSettings;
        private static Dictionary<KeySet, Action> _keybinds;

        public static bool IsEnable => _timer.Enabled;

        public static void Start()
        {
            _keyHook = new KeyboardHook();
            _keyHook.Hook();
            _keyHook.KeyDown += _keyHook_KeyDown;
            _timer = new Timer();
            _timer.Interval = 100d / 6d;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
            _userSettings = UserSettings.Instance;
            _mouseSettings = MouseEmulationSettings.Instance;
            _keybinds = new Dictionary<KeySet, Action>();
            _keybinds.Add(new KeySet(_userSettings.Quit), () => { Stop(); PluginManager.Instance.Exit(); });
            _keybinds.Add(new KeySet(_userSettings.Back), () => { Stop(); PluginManager.Instance.Undo(); });
            _keybinds.Add(new KeySet(_userSettings.Usage), () =>
            {
                if (_usagePanel == null)
                {
                    UIAssistantAPI.UIDispatcher.Invoke(() =>
                    {
                        _usagePanel = new Usage();
                        UIAssistantAPI.AddPanel(_usagePanel);
                        Finished += RemoveUsagePanel;
                    });
                }
                else
                {
                    RemoveUsagePanel();
                    Finished -= RemoveUsagePanel;
                }
            });
            UIAssistantAPI.AddTargetingReticle();
        }

        private static void _keyHook_KeyDown(object sender, LowLevelKeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            var keysState = e.PressedKeys;
            if (_keybinds.ContainsKey(keysState))
            {
                _keybinds[keysState].Invoke();
            }
            e.Handled = true;
        }

        private static void RemoveUsagePanel()
        {
            UIAssistantAPI.RemovePanel(_usagePanel);
            _usagePanel = null;
        }

        private static Usage _usagePanel;

        static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var moveDelta = 4;
            if (_keyHook.IsPressed(_mouseSettings.SlowDown.Key))
            {
                moveDelta = 1;
            }
            if (_keyHook.IsPressed(_mouseSettings.SpeedUp.Key))
            {
                moveDelta = 8;
            }

            var location = new Point(0, 0);
            if (_keyHook.IsPressed(_mouseSettings.Left.Key))
            {
                location.X -= moveDelta;
            }
            if (_keyHook.IsPressed(_mouseSettings.Right.Key))
            {
                location.X += moveDelta;
            }
            if (_keyHook.IsPressed(_mouseSettings.Up.Key))
            {
                location.Y -= moveDelta;
            }
            if (_keyHook.IsPressed(_mouseSettings.Down.Key))
            {
                location.Y += moveDelta;
            }
            if (location.X != 0 || location.Y != 0)
            {
                MouseOperation.DoMouseEventRelative(location.X, location.Y);
                UIAssistantAPI.MoveTargetingReticle(location.X, location.Y);
            }
            Click(_mouseSettings.Click.Key, MouseOperation.LeftDown, MouseOperation.LeftUp, ref _isPressedLbutton);
            Click(_mouseSettings.RightClick.Key, MouseOperation.RightDown, MouseOperation.RightUp, ref _isPressedRbutton);
            Click(_mouseSettings.MiddleClick.Key, MouseOperation.MiddleDown, MouseOperation.MiddleUp, ref _isPressedMbutton);
            if (_keyHook.IsPressed(_mouseSettings.WheelUp.Key))
            {
                MouseOperation.DoWheelEvent(60, MouseOperation.WheelOrientation.Vertical);
            }
            if (_keyHook.IsPressed(_mouseSettings.WheelDown.Key))
            {
                MouseOperation.DoWheelEvent(-60, MouseOperation.WheelOrientation.Vertical);
            }
            if (_keyHook.IsPressed(_mouseSettings.HWheelUp.Key))
            {
                MouseOperation.DoWheelEvent(-180, MouseOperation.WheelOrientation.Horizontal);
            }
            if (_keyHook.IsPressed(_mouseSettings.HWheelDown.Key))
            {
                MouseOperation.DoWheelEvent(180, MouseOperation.WheelOrientation.Horizontal);
            }
        }

        static void Click(Key key, Action down, Action up, ref bool isPressed)
        {
            var currentIsPress = _keyHook.IsPressed(key);
            if (!isPressed && currentIsPress)
            {
                isPressed = true;
                down();
            }
            else if (isPressed && !currentIsPress)
            {
                isPressed = false;
                up();
            }
        }

        public static void Stop()
        {
            if (_timer.Enabled)
            {
                _keyHook.Dispose();
                _timer.Stop();
                _timer.Enabled = false;
            }
            Finished?.Invoke();
            Finished = null;
            _userSettings = null;
            _keybinds = null;
            UIAssistantAPI.RemoveTargetingReticle();
        }
    }
}
