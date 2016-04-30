using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.SpatialNavigation
{
    class DistanceCaluculatorFunction
    {
        public delegate double DistanceCaluculator(Rect target, Rect focusedBounds, double weightingValue);

        public static DistanceCaluculator Up => (target, focusedBounds, weightingValue) =>
        {
            var start = focusedBounds.TopLeft;
            var end = focusedBounds.TopRight;
            var targetPoint = target.BottomCenter();
            return CaluculateDistanceVertical(start, end, target, targetPoint, focusedBounds, weightingValue);
        };

        public static DistanceCaluculator Down => (target, focusedBounds, weightingValue) =>
        {
            var start = focusedBounds.BottomLeft;
            var end = focusedBounds.BottomRight;
            var targetPoint = target.TopCenter();
            return CaluculateDistanceVertical(start, end, target, targetPoint, focusedBounds, weightingValue);
        };

        private static double WeightVertical(Point targetPoint, Point nearestCorner, double weightingValue)
        {
            var deltaY = Math.Max(Math.Abs(targetPoint.Y - nearestCorner.Y), 1);
            if (Math.Abs((targetPoint.X - nearestCorner.X) / deltaY) > 1)
            {
                return weightingValue * 2;
            }
            else
            {
                return weightingValue;
            }
        }

        private static double CaluculateDistanceVertical(Point start, Point end, Rect target, Point targetPoint, Rect focusedBounds, double weightingValue)
        {
            var evaluationValue = 0d;
            Point? nearestCorner = null;
            if ((target.Left < start.X && start.X < target.Right) || (target.Left < end.X && end.X < target.Right))
            {
                nearestCorner = null;
            }
            else if (targetPoint.X < start.X)
            {
                nearestCorner = start;
            }
            else if (end.X < targetPoint.X)
            {
                nearestCorner = end;
            }

            if (nearestCorner != null)
            {
                evaluationValue = WeightVertical(targetPoint, nearestCorner.Value, weightingValue);
            }

            evaluationValue += targetPoint.GetDistance(start, end);
            return evaluationValue;
        }

        public static DistanceCaluculator Left => (target, focusedBounds, weightingValue) =>
        {
            var start = focusedBounds.TopLeft;
            var end = focusedBounds.BottomLeft;
            var targetPoint = target.RightCenter();
            return CaluculateDistanceHorizontal(start, end, target, targetPoint, focusedBounds, weightingValue);
        };

        public static DistanceCaluculator Right => (target, focusedBounds, weightingValue) =>
        {
            var start = focusedBounds.TopRight;
            var end = focusedBounds.BottomRight;
            var targetPoint = target.LeftCenter();
            return CaluculateDistanceHorizontal(start, end, target, targetPoint, focusedBounds, weightingValue);
        };

        private static double WeightHorizontal(Point targetPoint, Point nearestCorner, double weightingValue)
        {
            var deltaX = Math.Max(Math.Abs(targetPoint.X - nearestCorner.X), 1);
            if (Math.Abs((targetPoint.Y - nearestCorner.Y) / deltaX) > 1)
            {
                return weightingValue * 2;
            }
            else
            {
                return weightingValue;
            }
        }

        private static double CaluculateDistanceHorizontal(Point start, Point end, Rect target, Point targetPoint, Rect focusedBounds, double weightingValue)
        {
            var evaluationValue = 0d;
            Point? nearestCorner = null;
            if ((target.Top < start.Y && start.Y < target.Bottom) || (target.Top < end.Y && end.Y < target.Bottom))
            {
                nearestCorner = null;
            }
            else if (targetPoint.Y < start.Y)
            {
                nearestCorner = start;
            }
            else if (end.Y < targetPoint.Y)
            {
                nearestCorner = end;
            }

            if (nearestCorner != null)
            {
                evaluationValue = WeightHorizontal(targetPoint, nearestCorner.Value, weightingValue);
            }

            evaluationValue += targetPoint.GetDistance(start, end);
            return evaluationValue;
        }
    }
}
