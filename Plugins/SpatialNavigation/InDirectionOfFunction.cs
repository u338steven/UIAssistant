using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace UIAssistant.Plugin.SpatialNavigation
{
    class InDirectionOfFunction
    {
        const double _margin_threshold = 3;

        public delegate bool InDirectionOf(Rect target, Rect focusedBounds);
        public static InDirectionOf Up => (target, focusedBounds) =>
        {
            if (focusedBounds.Top < target.Bottom - _margin_threshold)
                return false;
            return true;
        };

        public static InDirectionOf Down => (target, focusedBounds) =>
        {
            if (focusedBounds.Bottom - _margin_threshold > target.Top)
                return false;
            return true;
        };

        public static InDirectionOf Left => (target, focusedBounds) =>
        {
            if (focusedBounds.Left < target.Right - _margin_threshold)
                return false;
            return true;
        };

        public static InDirectionOf Right => (target, focusedBounds) =>
        {
            if (focusedBounds.Right - _margin_threshold > target.Left)
                return false;
            return true;
        };
    }
}
