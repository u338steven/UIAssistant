using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Settings;
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
        static IUserSettings _userSettings;
        static MouseEmulationSettings _mouseSettings;
        private static IKeybindManager _keybinds;

        public static bool IsEnable => _timer.Enabled;

        public static void Start()
        {
            var api = MouseEmulation.UIAssistantAPI;
            var handlers = api.KeyboardAPI.CreateHookHandlers();
            handlers.KeyDown += Handlers_KeyDown;
            api.KeyboardAPI.Hook(handlers);
            Finished += () => api.KeyboardAPI.Unhook(handlers);
            _timer = new Timer();
            _timer.Interval = 100d / 6d;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
            _userSettings = MouseEmulation.UIAssistantAPI.UIAssistantSettings;
            _mouseSettings = MouseEmulation.Settings;
            _keybinds = api.KeyboardAPI.CreateKeybindManager();
            _keybinds.Add(_userSettings.Quit, () => { Stop(); MouseEmulation.UIAssistantAPI.PluginManager.Exit(); });
            _keybinds.Add(_userSettings.Back, () => { Stop(); MouseEmulation.UIAssistantAPI.PluginManager.Undo(); });
            _keybinds.Add(_userSettings.Usage, () =>
            {
                if (_usagePanel == null)
                {
                    MouseEmulation.UIAssistantAPI.ViewAPI.UIDispatcher.Invoke(() =>
                    {
                        _usagePanel = new Usage();
                        MouseEmulation.UIAssistantAPI.ViewAPI.AddPanel(_usagePanel);
                        Finished += RemoveUsagePanel;
                    });
                }
                else
                {
                    RemoveUsagePanel();
                    Finished -= RemoveUsagePanel;
                }
            });
            MouseEmulation.UIAssistantAPI.ViewAPI.AddTargetingReticle();
        }

        private static void Handlers_KeyDown(object sender, LowLevelKeyEventArgs e)
        {
            _keybinds.Execute(e.PressedKeys, e.CurrentKeyInfo.IsKeyHoldDown);
            e.Handled = true;
        }

        private static void RemoveUsagePanel()
        {
            MouseEmulation.UIAssistantAPI.ViewAPI.RemovePanel(_usagePanel);
            _usagePanel = null;
        }

        private static Usage _usagePanel;

        static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var api = MouseEmulation.UIAssistantAPI.KeyboardAPI;

            var moveDelta = 4;
            var operation = MouseEmulation.UIAssistantAPI.MouseAPI.MouseOperation;
            if (api.IsPressed(_mouseSettings.SlowDown.Key))
            {
                moveDelta = 1;
            }
            if (api.IsPressed(_mouseSettings.SpeedUp.Key))
            {
                moveDelta = 8;
            }

            var location = new Point(0, 0);
            if (api.IsPressed(_mouseSettings.Left.Key))
            {
                location.X -= moveDelta;
            }
            if (api.IsPressed(_mouseSettings.Right.Key))
            {
                location.X += moveDelta;
            }
            if (api.IsPressed(_mouseSettings.Up.Key))
            {
                location.Y -= moveDelta;
            }
            if (api.IsPressed(_mouseSettings.Down.Key))
            {
                location.Y += moveDelta;
            }
            if (location.X != 0 || location.Y != 0)
            {
                operation.DoMouseEventRelative(location.X, location.Y);
                MouseEmulation.UIAssistantAPI.ViewAPI.MoveTargetingReticle(location.X, location.Y);
            }
            Click(_mouseSettings.Click.Key, operation.LeftDown, operation.LeftUp, ref _isPressedLbutton);
            Click(_mouseSettings.RightClick.Key, operation.RightDown, operation.RightUp, ref _isPressedRbutton);
            Click(_mouseSettings.MiddleClick.Key, operation.MiddleDown, operation.MiddleUp, ref _isPressedMbutton);
            if (api.IsPressed(_mouseSettings.WheelUp.Key))
            {
                operation.DoWheelEvent(60, WheelOrientation.Vertical);
            }
            if (api.IsPressed(_mouseSettings.WheelDown.Key))
            {
                operation.DoWheelEvent(-60, WheelOrientation.Vertical);
            }
            if (api.IsPressed(_mouseSettings.HWheelUp.Key))
            {
                operation.DoWheelEvent(-180, WheelOrientation.Horizontal);
            }
            if (api.IsPressed(_mouseSettings.HWheelDown.Key))
            {
                operation.DoWheelEvent(180, WheelOrientation.Horizontal);
            }
        }

        static void Click(Key key, Action down, Action up, ref bool isPressed)
        {
            var api = MouseEmulation.UIAssistantAPI.KeyboardAPI;
            var currentIsPress = api.IsPressed(key);
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
                _timer.Stop();
                _timer.Enabled = false;
            }
            Finished?.Invoke();
            Finished = null;
            _userSettings = null;
            _keybinds = null;
            MouseEmulation.UIAssistantAPI.ViewAPI.RemoveTargetingReticle();
        }
    }
}
