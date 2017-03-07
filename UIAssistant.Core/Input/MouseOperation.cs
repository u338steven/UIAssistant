using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Windows;

using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Native;

namespace UIAssistant.Core.Input
{
    public class MouseOperation : IMouseOperation
    {
        public void Move(double x, double y)
        {
            DoMouseEvent(x, y);
        }

        public void Move(Point pt)
        {
            Move(pt.X, pt.Y);
        }

        public void Move(Rect bounds)
        {
            Move(bounds.Center());
        }

        public void MoveTo(Point from, Point to, int millisecondsInterval = 50, int count = 10)
        {
            var deltaX = Math.Max((to.X - from.X) / count, 1);
            var deltaY = Math.Max((to.Y - from.Y) / count, 1);
            for (int i = 0; i < count; ++i)
            {
                DoMouseEvent(from.X + deltaX * i, from.Y + deltaY * i);
            }
            DoMouseEvent(to);
        }

        public void LeftDown()
        {
            DoMouseEvent(MouseEvent.MOUSEEVENTF_LEFTDOWN);
        }

        public void RightDown()
        {
            DoMouseEvent(MouseEvent.MOUSEEVENTF_RIGHTDOWN);
        }

        public void MiddleDown()
        {
            DoMouseEvent(MouseEvent.MOUSEEVENTF_MIDDLEDOWN);
        }

        public void LeftUp()
        {
            DoMouseEvent(MouseEvent.MOUSEEVENTF_LEFTUP);
        }

        public void RightUp()
        {
            DoMouseEvent(MouseEvent.MOUSEEVENTF_RIGHTUP);
        }

        public void MiddleUp()
        {
            DoMouseEvent(MouseEvent.MOUSEEVENTF_MIDDLEUP);
        }

        public void Click(Rect bounds)
        {
            Move(bounds);
            DoMouseEvent(MouseEvent.MOUSEEVENTF_LEFTDOWN, MouseEvent.MOUSEEVENTF_LEFTUP);
        }

        public void RightClick(Rect bounds)
        {
            Move(bounds);
            DoMouseEvent(MouseEvent.MOUSEEVENTF_RIGHTDOWN, MouseEvent.MOUSEEVENTF_RIGHTUP);
        }

        public void MiddleClick(Rect bounds)
        {
            Move(bounds);
            DoMouseEvent(MouseEvent.MOUSEEVENTF_MIDDLEDOWN, MouseEvent.MOUSEEVENTF_MIDDLEUP);
        }

        public void DoubleClick(Rect bounds)
        {
            Move(bounds);
            DoMouseEvent(MouseEvent.MOUSEEVENTF_LEFTDOWN, MouseEvent.MOUSEEVENTF_LEFTUP, MouseEvent.MOUSEEVENTF_LEFTDOWN, MouseEvent.MOUSEEVENTF_LEFTUP);
        }

        public void Drag(Rect bounds)
        {
            var center = bounds.Center();
            Task.Run(() =>
            {
                Move(center);
                DoMouseEvent(MouseEvent.MOUSEEVENTF_LEFTDOWN);
                System.Threading.Thread.Sleep(100);
                Enumerable.Range(1, 20).ForEach(i =>
                {
                    center.Offset(-1, -1);
                    Move(center);
                    System.Threading.Thread.Sleep(10);
                });
            });
        }

        public void Drop(Rect bounds)
        {
            var center = bounds.Center();
            center.Offset(20, 20);
            Enumerable.Range(1, 20).ForEach(i =>
            {
                center.Offset(-1, -1);
                Move(center);
                System.Threading.Thread.Sleep(10);
            });
            DoMouseEvent(MouseEvent.MOUSEEVENTF_LEFTUP);
        }

        public void DoMouseEvent(params MouseEvent[] dwFlags)
        {
            var inputs = new List<INPUT>();

            foreach (var operation in dwFlags)
            {
                var mouse = new INPUT();
                mouse.type = InputKind.INPUT_MOUSE;
                mouse.iu.mi.dwFlags = operation;
                inputs.Add(mouse);
            }

            NativeMethods.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        public void DoMouseEventRelative(double x, double y, params MouseEvent[] dwFlags)
        {
            var inputs = new List<INPUT>();

            var input = new INPUT();
            input.type = InputKind.INPUT_MOUSE;
            input.iu.mi.dwFlags = MouseEvent.MOUSEEVENTF_MOVED;
            input.iu.mi.dx = (int)x;
            input.iu.mi.dy = (int)y;
            inputs.Add(input);

            foreach (var operation in dwFlags)
            {
                var mouse = new INPUT();
                mouse.type = InputKind.INPUT_MOUSE;
                mouse.iu.mi.dwFlags = operation;
                inputs.Add(mouse);
            }

            NativeMethods.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        public void DoMouseEvent(Point point, params MouseEvent[] dwFlags)
        {
            DoMouseEvent(point.X, point.Y, dwFlags);
        }

        public void DoMouseEvent(Rect bounds, params MouseEvent[] dwFlags)
        {
            DoMouseEvent(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2, dwFlags);
        }

        public void DoMouseEvent(double x, double y, params MouseEvent[] dwFlags)
        {
            var inputs = new List<INPUT>();

            var input = new INPUT();
            input.type = InputKind.INPUT_MOUSE;
            input.iu.mi.dwFlags = MouseEvent.MOUSEEVENTF_MOVED | MouseEvent.MOUSEEVENTF_ABSOLUTE;
            input.iu.mi.dx = (int)((x + SystemParameters.VirtualScreenLeft) * (65536.0 / SystemParameters.PrimaryScreenWidth));
            input.iu.mi.dy = (int)((y + SystemParameters.VirtualScreenTop) * (65536.0 / SystemParameters.PrimaryScreenHeight));
            inputs.Add(input);

            foreach (var operation in dwFlags)
            {
                var mouse = new INPUT();
                mouse.type = InputKind.INPUT_MOUSE;
                mouse.iu.mi.dwFlags = operation;
                inputs.Add(mouse);
            }

            NativeMethods.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        public void DoWheelEvent(int amountOfMovement, params WheelOrientation[] orientations)
        {
            var inputs = new List<INPUT>();

            foreach (var orientation in orientations)
            {
                var mouse = new INPUT();
                mouse.type = InputKind.INPUT_MOUSE;
                if (orientation == WheelOrientation.Horizontal)
                {
                    mouse.iu.mi.dwFlags = MouseEvent.MOUSEEVENTF_HWHEEL;
                }
                else
                {
                    mouse.iu.mi.dwFlags = MouseEvent.MOUSEEVENTF_WHEEL;
                }
                mouse.iu.mi.mouseData = amountOfMovement;
                inputs.Add(mouse);
            }

            NativeMethods.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
        }

        public Point GetMousePosition()
        {
            POINT pt = new POINT();
            NativeMethods.GetCursorPos(ref pt);
            return new Point(pt.x, pt.y);
        }
    }
}
