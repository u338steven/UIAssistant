using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Windows;

using UIAssistant.Utility.Win32;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Core.Input
{
    public class MouseOperation
    {
        public static void Move(double x, double y)
        {
            DoMouseEvent(x, y);
        }

        public static void Move(Point pt)
        {
            Move(pt.X, pt.Y);
        }

        public static void Move(Rect bounds)
        {
            Move(bounds.Center());
        }

        public static void MoveTo(Point from, Point to, int millisecondsInterval = 50, int count = 10)
        {
            var deltaX = Math.Max((to.X - from.X) / count, 1);
            var deltaY = Math.Max((to.Y - from.Y) / count, 1);
            for (int i = 0; i < count; ++i)
            {
                DoMouseEvent(from.X + deltaX * i, from.Y + deltaY * i);
            }
            DoMouseEvent(to);
        }

        public static void LeftDown()
        {
            DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_LEFTDOWN);
        }

        public static void RightDown()
        {
            DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_RIGHTDOWN);
        }

        public static void MiddleDown()
        {
            DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_MIDDLEDOWN);
        }

        public static void LeftUp()
        {
            DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_LEFTUP);
        }

        public static void RightUp()
        {
            DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_RIGHTUP);
        }

        public static void MiddleUp()
        {
            DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_MIDDLEUP);
        }

        public static void Click(Rect bounds)
        {
            Move(bounds);
            DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_LEFTDOWN, Win32Interop.MouseEvent.MOUSEEVENTF_LEFTUP);
        }

        public static void RightClick(Rect bounds)
        {
            Move(bounds);
            DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_RIGHTDOWN, Win32Interop.MouseEvent.MOUSEEVENTF_RIGHTUP);
        }

        public static void MiddleClick(Rect bounds)
        {
            Move(bounds);
            DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_MIDDLEDOWN, Win32Interop.MouseEvent.MOUSEEVENTF_MIDDLEUP);
        }

        public static void DoubleClick(Rect bounds)
        {
            Move(bounds);
            DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_LEFTDOWN, Win32Interop.MouseEvent.MOUSEEVENTF_LEFTUP, Win32Interop.MouseEvent.MOUSEEVENTF_LEFTDOWN, Win32Interop.MouseEvent.MOUSEEVENTF_LEFTUP);
        }

        public static void Drag(Rect bounds)
        {
            var center = bounds.Center();
            Task.Run(() =>
            {
                Move(center);
                DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_LEFTDOWN);
                System.Threading.Thread.Sleep(100);
                Enumerable.Range(1, 20).ForEach(i =>
                {
                    center.Offset(-1, -1);
                    Move(center);
                    System.Threading.Thread.Sleep(10);
                });
            });
        }

        public static void Drop(Rect bounds)
        {
            var center = bounds.Center();
            center.Offset(20, 20);
            Enumerable.Range(1, 20).ForEach(i =>
            {
                center.Offset(-1, -1);
                Move(center);
                System.Threading.Thread.Sleep(10);
            });
            DoMouseEvent(Win32Interop.MouseEvent.MOUSEEVENTF_LEFTUP);
        }

        public static void DoMouseEvent(params Win32Interop.MouseEvent[] dwFlags)
        {
            var inputs = new List<Win32Interop.INPUT>();

            foreach (var operation in dwFlags)
            {
                var mouse = new Win32Interop.INPUT();
                mouse.type = Win32Interop.INPUT_MOUSE;
                mouse.iu.mi.dwFlags = operation;
                inputs.Add(mouse);
            }

            Win32Interop.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        public static void DoMouseEventRelative(double x, double y, params Win32Interop.MouseEvent[] dwFlags)
        {
            var inputs = new List<Win32Interop.INPUT>();

            var input = new Win32Interop.INPUT();
            input.type = Win32Interop.INPUT_MOUSE;
            input.iu.mi.dwFlags = Win32Interop.MouseEvent.MOUSEEVENTF_MOVED;
            input.iu.mi.dx = (int)x;
            input.iu.mi.dy = (int)y;
            inputs.Add(input);

            foreach (var operation in dwFlags)
            {
                var mouse = new Win32Interop.INPUT();
                mouse.type = Win32Interop.INPUT_MOUSE;
                mouse.iu.mi.dwFlags = operation;
                inputs.Add(mouse);
            }

            Win32Interop.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        public static void DoMouseEvent(Point point, params Win32Interop.MouseEvent[] dwFlags)
        {
            DoMouseEvent(point.X, point.Y, dwFlags);
        }

        public static void DoMouseEvent(Rect bounds, params Win32Interop.MouseEvent[] dwFlags)
        {
            DoMouseEvent(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2, dwFlags);
        }

        public static void DoMouseEvent(double x, double y, params Win32Interop.MouseEvent[] dwFlags)
        {
            var inputs = new List<Win32Interop.INPUT>();

            var input = new Win32Interop.INPUT();
            input.type = Win32Interop.INPUT_MOUSE;
            input.iu.mi.dwFlags = Win32Interop.MouseEvent.MOUSEEVENTF_MOVED | Win32Interop.MouseEvent.MOUSEEVENTF_ABSOLUTE;
            input.iu.mi.dx = (int)((x + SystemParameters.VirtualScreenLeft) * (65536.0 / SystemParameters.PrimaryScreenWidth));
            input.iu.mi.dy = (int)((y + SystemParameters.VirtualScreenTop) * (65536.0 / SystemParameters.PrimaryScreenHeight));
            inputs.Add(input);

            foreach (var operation in dwFlags)
            {
                var mouse = new Win32Interop.INPUT();
                mouse.type = Win32Interop.INPUT_MOUSE;
                mouse.iu.mi.dwFlags = operation;
                inputs.Add(mouse);
            }

            Win32Interop.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        public enum WheelOrientation
        {
            Vertical,
            Horizontal,
        }

        public static void DoWheelEvent(int amountOfMovement, params WheelOrientation[] orientations)
        {
            var inputs = new List<Win32Interop.INPUT>();

            foreach (var orientation in orientations)
            {
                var mouse = new Win32Interop.INPUT();
                mouse.type = Win32Interop.INPUT_MOUSE;
                if (orientation == WheelOrientation.Horizontal)
                {
                    mouse.iu.mi.dwFlags = Win32Interop.MouseEvent.MOUSEEVENTF_HWHEEL;
                }
                else
                {
                    mouse.iu.mi.dwFlags = Win32Interop.MouseEvent.MOUSEEVENTF_WHEEL;
                }
                mouse.iu.mi.mouseData = amountOfMovement;
                inputs.Add(mouse);
            }

            Win32Interop.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        public static Point GetMousePosition()
        {
            Win32Interop.Point pt = new Win32Interop.Point();
            Win32Interop.GetCursorPos(ref pt);
            return new Point(pt.x, pt.y);
        }
    }
}
