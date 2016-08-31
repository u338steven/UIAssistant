using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Windows.Input;

using UIAssistant.Utility.Win32;

namespace UIAssistant.Core.Input
{
    public class KeyboardOperation
    {
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int vkkey);

        public static void SendKeys(params Key[] keys)
        {
            if (keys == null)
            {
                return;
            }

            lock (_forRestore)
            {
                var inputs = new List<Win32Interop.INPUT>();

                foreach (var key in _forRestore)
                {
                    inputs.Add(GenerateKeyUp(key));
                }

                List<Win32Interop.INPUT> keyups = new List<Win32Interop.INPUT>();
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

                Win32Interop.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
                _forRestore.Clear();
            }
        }

        private static bool IsExtended(Key key)
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

        static List<Key> _forRestore = new List<Key>();
        static bool _cancelAlt = false;

        public static void Initialize(params Key[] keys)
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

        static Key _cancelKey = Key.NoName;
        public static void CancelAltKey()
        {
            KeyDown(_cancelKey);
            KeyUp(_cancelKey);
            _cancelAlt = true;
        }

        public static void PressedKeyUp()
        {
            var pressedKeys = GetPressedKey().ToArray();
            KeyUp(pressedKeys);
        }

        private static Key[] _modifiers = { Key.LeftAlt, Key.RightAlt, Key.LeftCtrl, Key.RightCtrl, Key.LeftShift, Key.RightShift };
        private static List<Key> GetPressedKey()
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

        private static bool IsDown(int vk)
        {
            if (GetKeyState(vk) != 0)
            {
                return true;
            }
            return false;
        }

        public static void KeyUp(params Key[] keys)
        {
            if (keys == null || keys.Length <= 0)
            {
                return;
            }
            var inputs = new List<Win32Interop.INPUT>();
            foreach (var key in keys)
            {
                var keyup = GenerateKeyUp(key);
                inputs.Add(keyup);
            }
            Win32Interop.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        public static void KeyDown(params Key[] keys)
        {
            if (keys == null || keys.Length <= 0)
            {
                return;
            }
            var inputs = new List<Win32Interop.INPUT>();
            foreach (var key in keys)
            {
                var keydown = GenerateKeyDown(key);
                inputs.Add(keydown);
            }
            Win32Interop.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        private static Win32Interop.INPUT GenerateKeyUp(Key key)
        {
            var keyup = new Win32Interop.INPUT();
            keyup.type = Win32Interop.INPUT_KEYBOARD;
            keyup.iu.ki.wVk = (short)KeyInterop.VirtualKeyFromKey(key);
            keyup.iu.ki.wScan = (short)MapVirtualKey(keyup.iu.ki.wVk, 0);
            keyup.iu.ki.dwFlags = Win32Interop.KeyEvent.KEYEVENTF_KEYUP | (IsExtended(key) ? Win32Interop.KeyEvent.KEYEVENTF_EXTENDEDKEY : 0);
            keyup.iu.ki.dwExtraInfo = GetMessageExtraInfo();

            return keyup;
        }

        private static Win32Interop.INPUT GenerateKeyDown(Key key)
        {
            var keydown = new Win32Interop.INPUT();
            keydown.type = Win32Interop.INPUT_KEYBOARD;
            keydown.iu.ki.wVk = (short)KeyInterop.VirtualKeyFromKey(key);
            keydown.iu.ki.wScan = (short)MapVirtualKey(keydown.iu.ki.wVk, 0);
            keydown.iu.ki.dwFlags = Win32Interop.KeyEvent.KEYEVENTF_KEYDOWN | (IsExtended(key) ? Win32Interop.KeyEvent.KEYEVENTF_EXTENDEDKEY : 0);
            keydown.iu.ki.dwExtraInfo = GetMessageExtraInfo();

            return keydown;
        }

        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(int wCode, int wMapType);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();
    }
}
