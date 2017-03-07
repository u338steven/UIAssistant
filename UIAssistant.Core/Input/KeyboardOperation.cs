using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;

using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Native;

namespace UIAssistant.Core.Input
{
    public class KeyboardOperation : IKeyboardOperation
    {
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int vkkey);

        public void SendKeys(params Key[] keys)
        {
            if (keys == null)
            {
                return;
            }

            lock (_forRestore)
            {
                var inputs = new List<INPUT>();

                foreach (var key in _forRestore)
                {
                    inputs.Add(GenerateKeyUp(key));
                }

                List<INPUT> keyups = new List<INPUT>();
                foreach (var key in keys)
                {
                    if (key == Key.None)
                    {
                        keyups.Reverse();
                        inputs.AddRange(keyups);
                        keyups.Clear();
                        continue;
                    }

                    inputs.Add(GenerateKeyDown(key));
                    keyups.Add(GenerateKeyUp(key));
                }
                keyups.Reverse();
                inputs.AddRange(keyups);

                foreach (var key in _forRestore)
                {
                    inputs.Add(GenerateKeyDown(key));
                }

                if (_cancelAlt)
                {
                    inputs.Add(GenerateKeyDown(_cancelKey));
                    inputs.Add(GenerateKeyUp(_cancelKey));
                }

                NativeMethods.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
                _forRestore.Clear();
            }
        }

        private bool IsExtended(Key key)
        {
            switch (key)
            {
                case Key.Pause:
                case Key.NumLock:
                case Key.Insert:
                case Key.Delete:
                case Key.PageUp:
                case Key.PageDown:
                case Key.End:
                case Key.Home:
                case Key.Left:
                case Key.Up:
                case Key.Right:
                case Key.Down:
                case Key.Print:
                case Key.RightShift:
                case Key.RightCtrl:
                case Key.RightAlt:
                    return true;
                default:
                    break;
            }
            return false;
        }

        List<Key> _forRestore = new List<Key>();
        bool _cancelAlt = false;

        public void Initialize(params Key[] keys)
        {
            lock (_forRestore)
            {
                _cancelAlt = false;
                _forRestore = new List<Key>(keys);
                if (_forRestore.Contains(Key.LeftAlt) || _forRestore.Contains(Key.RightAlt))
                {
                    CancelAltKey();
                }
            }
        }

        Key _cancelKey = Key.NoName;
        public void CancelAltKey()
        {
            KeyDown(_cancelKey);
            KeyUp(_cancelKey);
            _cancelAlt = true;
        }

        public void PressedKeyUp()
        {
            var pressedKeys = GetPressedKey().ToArray();
            KeyUp(pressedKeys);
        }

        private Key[] _modifiers = { Key.LeftAlt, Key.RightAlt, Key.LeftCtrl, Key.RightCtrl, Key.LeftShift, Key.RightShift };
        private List<Key> GetPressedKey()
        {
            var result = new List<Key>();
            foreach (var x in _modifiers)
            {
                if (IsDown(KeyInterop.VirtualKeyFromKey(x)))
                {
                    result.Add(x);
                }
            }
            return result;
        }

        private bool IsDown(int vk)
        {
            if (GetKeyState(vk) != 0)
            {
                return true;
            }
            return false;
        }

        public void KeyUp(params Key[] keys)
        {
            if (keys == null || keys.Length <= 0)
            {
                return;
            }
            var inputs = new List<INPUT>();
            foreach (var key in keys)
            {
                var keyup = GenerateKeyUp(key);
                inputs.Add(keyup);
            }
            NativeMethods.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        public void KeyDown(params Key[] keys)
        {
            if (keys == null || keys.Length <= 0)
            {
                return;
            }
            var inputs = new List<INPUT>();
            foreach (var key in keys)
            {
                var keydown = GenerateKeyDown(key);
                inputs.Add(keydown);
            }
            NativeMethods.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        private INPUT GenerateKeyUp(Key key)
        {
            var keyup = new INPUT();
            keyup.type = InputKind.INPUT_KEYBOARD;
            keyup.iu.ki.wVk = (short)KeyInterop.VirtualKeyFromKey(key);
            keyup.iu.ki.wScan = (short)MapVirtualKey(keyup.iu.ki.wVk, 0);
            keyup.iu.ki.dwFlags = KeyEvent.KEYEVENTF_KEYUP | (IsExtended(key) ? KeyEvent.KEYEVENTF_EXTENDEDKEY : 0);
            keyup.iu.ki.dwExtraInfo = GetMessageExtraInfo();

            return keyup;
        }

        private INPUT GenerateKeyDown(Key key)
        {
            var keydown = new INPUT();
            keydown.type = InputKind.INPUT_KEYBOARD;
            keydown.iu.ki.wVk = (short)KeyInterop.VirtualKeyFromKey(key);
            keydown.iu.ki.wScan = (short)MapVirtualKey(keydown.iu.ki.wVk, 0);
            keydown.iu.ki.dwFlags = KeyEvent.KEYEVENTF_KEYDOWN | (IsExtended(key) ? KeyEvent.KEYEVENTF_EXTENDEDKEY : 0);
            keydown.iu.ki.dwExtraInfo = GetMessageExtraInfo();

            return keydown;
        }

        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(int wCode, int wMapType);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();
    }
}
