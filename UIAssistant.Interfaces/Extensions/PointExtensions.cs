namespace System.Windows
{
    public static class PointExtensions
    {
        public static double GetDistance(this Point p, Point q)
        {
            var x = p.X - q.X;
            var y = p.Y - q.Y;
            return Math.Sqrt(x * x + y * y);
        }

        public static double GetDistance(this Point p, Point lineSegmentFrom, Point lineSegmentTo)
        {
            var lineSegmentDistance = GetDistance(lineSegmentFrom, lineSegmentTo);
            if (lineSegmentDistance == 0) return GetDistance(p, lineSegmentFrom);
            var t = ((p.X - lineSegmentFrom.X) * (lineSegmentTo.X - lineSegmentFrom.X) + (p.Y - lineSegmentFrom.Y) * (lineSegmentTo.Y - lineSegmentFrom.Y)) / (lineSegmentDistance * lineSegmentDistance);
            t = Math.Max(0, Math.Min(1, t));
            return GetDistance(p, new Point(lineSegmentFrom.X + t * (lineSegmentTo.X - lineSegmentFrom.X), lineSegmentFrom.Y + t * (lineSegmentTo.Y - lineSegmentFrom.Y)));
        }
    }
}
