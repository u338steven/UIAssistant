using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using System.Windows.Automation;
using System.Windows.Automation.Text;

using UIAssistant.Core.Tools;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    class EmbeddedCommand
    {
        const int EM_GETSEL = 0x00B0;
        const int EM_SETSEL = 0x00B1;
        const int EM_SCROLLCARET = 0x00B7;
        const int EM_REPLACESEL = 0x00C2;

        const int EM_GETLINECOUNT = 0x00BA;
        const int EM_LINEINDEX = 0x00BB;
        const int EM_LINELENGTH = 0x00C1;
        const int EM_GETLINE = 0x00C4;
        const int EM_LINEFROMCHAR = 0x00C9;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SendMessage(IntPtr hWnd, uint Msg, out int wParam, out int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        public static int GetCaretPos()
        {
            var focused = AutomationElement.FocusedElement;
            if (focused == null)
            {
                return -1;
            }

            var controlType = focused.Current.ControlType;
            if (controlType != ControlType.Edit && controlType != ControlType.Document)
            {
                return -1;
            }
            var focusedHandle = focused.Current.NativeWindowHandle;

            int start = -1, end = -1;
            SendMessage(focusedHandle, EM_GETSEL, out start, out end);
            return start;
        }

        public static void Find(int startIndex, string target, bool backward = false, bool before = false, bool isVisualMode = false)
        {
            var focused = AutomationElement.FocusedElement;
            if (focused == null)
            {
                return;
            }

            var controlType = focused.Current.ControlType;
            if (controlType != ControlType.Edit && controlType != ControlType.Document)
            {
                return;
            }
            var focusedHandle = focused.Current.NativeWindowHandle;

            int start = -1, end = -1;
            SendMessage(focusedHandle, EM_GETSEL, out start, out end);

            if (start < 0 || end < 0)
            {
                // TextPattern: WPF
                FindInWPFEdit(startIndex, target, backward, before, isVisualMode);
                return;
            }

            if (backward && start == 0)
            {
                return;
            }

            var line = SendMessage(focusedHandle, EM_LINEFROMCHAR, -1, 0);
            var lineIndex = SendMessage(focusedHandle, EM_LINEINDEX, -1, 0);
            var length = SendMessage(focusedHandle, EM_LINELENGTH, lineIndex, 0);

            if (length == 0)
            {
                return;
            }

            // for UNICODE length
            IntPtr textPtr = Marshal.AllocHGlobal(length * 2);
            string text;
            try
            {
                var byteLength = BitConverter.GetBytes((short)length);
                Marshal.Copy(byteLength, 0, textPtr, 2);

                SendMessage(focusedHandle, EM_GETLINE, line, textPtr);
                text = Marshal.PtrToStringAuto(textPtr, length);
            }
            finally
            {
                Marshal.FreeHGlobal(textPtr);
            }

            //System.Diagnostics.Debug.Print($"{lineIndex}, {length}, {line}, {sb.ToString()}");

            var tmpStart = startIndex == start ? end : start;
            if (before)
            {
                if (backward)
                {
                    --tmpStart;
                }
                else
                {
                    ++tmpStart;
                }
            }
            // migemo
            //var regex = Migemo.GetRegex(target);
            //var match = regex.Match(text.ToString().Substring(start - lineIndex));

            //if (!match.Success)
            //{
            //    return;
            //}
            //var move = match.Index;
            // migemo
            if (!backward)
            {
                if (tmpStart - lineIndex < 0 || text.Length <= tmpStart - lineIndex)
                {
                    return;
                }
            }
            else
            {
                if (tmpStart - lineIndex - 1 < 0)
                {
                    return;
                }
            }
            var move = backward ? text.ToString().Substring(0, tmpStart - lineIndex - 1).LastIndexOf(target) : text.ToString().Substring(tmpStart - lineIndex).IndexOf(target);
            if (move == -1)
            {
                return;
            }
            //var adjuster = (before && !backward) ? 0 : 1;
            //var adjuster = before ? backward ? 1 : 0 : backward ? 0 : 1;
            var adjuster = before ? backward ? 1 : 0 : backward ? 1 : 1;
            var destination = backward ? move + lineIndex + adjuster : move + adjuster + tmpStart;
            if (!isVisualMode)
            {
                SendMessage(focusedHandle, EM_SETSEL, destination, destination);
            }
            else
            {
                SendMessage(focusedHandle, EM_SETSEL, startIndex, destination);
            }
            SendMessage(focusedHandle, EM_SCROLLCARET, 0, 0);
        }

        public static void FindInWPFEdit(int startIndex, string target, bool backward = false, bool before = false, bool isVisualMode = false)
        {
            var focused = AutomationElement.FocusedElement;
            if (focused == null)
            {
                return;
            }

            object textPatternObj;
            if (!focused.TryGetCurrentPattern(TextPattern.Pattern, out textPatternObj))
            {
                return;
            }

            var textPattern = textPatternObj as TextPattern;
            if (textPattern.SupportedTextSelection == SupportedTextSelection.None)
            {
                return;
            }
            var currentSelected = textPattern.GetSelection()[0];
            var lines = textPattern.GetVisibleRanges();
            string text = "";
            TextPatternRange currentLine = null;

            foreach (var line in lines)
            {
                var start = line.CompareEndpoints(TextPatternRangeEndpoint.Start, currentSelected, TextPatternRangeEndpoint.Start);
                var end = line.CompareEndpoints(TextPatternRangeEndpoint.End, currentSelected, TextPatternRangeEndpoint.Start);
                if (start <= 0 && end >= 0)
                {
                    currentLine = line;
                    text = line.GetText(-1);
                    break;
                }
            }
            //System.Diagnostics.Debug.Print($"{currentSelected[0].}");

            if (currentLine == null)
            {
                return;
            }

            if (backward)
            {
                currentSelected.MoveEndpointByRange(TextPatternRangeEndpoint.Start, currentLine, TextPatternRangeEndpoint.Start);
            }
            else
            {
                currentSelected.MoveEndpointByRange(TextPatternRangeEndpoint.End, currentLine, TextPatternRangeEndpoint.End);
            }
            var currentText = currentSelected.GetText(-1);
            if (currentText.Length == 0)
            {
                return;
            }
            var targetText = before ? backward ? currentText.Substring(0, currentText.Length - 1) : currentText.Substring(1, currentText.Length - 1) : backward ? currentText.Substring(0, currentText.Length - 1) : currentText;
            int index = -1;

            //System.Diagnostics.Debug.Print($"{targetText}, {before}, {backward}");
            if (backward)
            {
                index = targetText.LastIndexOf(target);
            }
            else
            {
                index = targetText.IndexOf(target);
            }
            if (index < 0)
            {
                return;
            }
            //var adjuster = before ? backward ? 1 : 1 : backward ? 0 : 1;
            var adjuster = before ? backward ? 1 : 1 : 1;
            //var adjuster = before ? backward ? 0 : 1 : 1;
            currentSelected.Move(TextUnit.Character, index + adjuster);
            if (!isVisualMode)
            {
                currentSelected.MoveEndpointByRange(TextPatternRangeEndpoint.End, currentSelected, TextPatternRangeEndpoint.Start);
            }
            //System.Diagnostics.Debug.Print($"move:{index + adjuster}");
            currentSelected.Select();
            //System.Diagnostics.Debug.Print($"{text}");
        }
    }
}
