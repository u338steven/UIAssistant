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
        public static void SendKeys(params Key[] keys)
        {
            var inputs = new List<Win32Interop.INPUT>();

            foreach (var key in keys)
            {
                var keydown = new Win32Interop.INPUT();
                keydown.type = Win32Interop.INPUT_KEYBOARD;
                keydown.iu.ki.wVk = (short)KeyInterop.VirtualKeyFromKey(key);
                keydown.iu.ki.wScan = (short)MapVirtualKey(keydown.iu.ki.wVk, 0);
                keydown.iu.ki.dwFlags = Win32Interop.KeyEvent.KEYEVENTF_KEYDOWN | Win32Interop.KeyEvent.KEYEVENTF_EXTENDEDKEY;
                keydown.iu.ki.dwExtraInfo = GetMessageExtraInfo();
                inputs.Add(keydown);

                var keyup = new Win32Interop.INPUT();
                keyup.type = Win32Interop.INPUT_KEYBOARD;
                keyup.iu.ki.wVk = (short)KeyInterop.VirtualKeyFromKey(key);
                keyup.iu.ki.wScan = (short)MapVirtualKey(keyup.iu.ki.wVk, 0);
                keyup.iu.ki.dwFlags = Win32Interop.KeyEvent.KEYEVENTF_KEYUP | Win32Interop.KeyEvent.KEYEVENTF_EXTENDEDKEY;
                keyup.iu.ki.dwExtraInfo = GetMessageExtraInfo();
                inputs.Add(keyup);
            }

            Win32Interop.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(int wCode, int wMapType);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();
    }
}
