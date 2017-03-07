namespace System.Windows
{
    public static class RectExtensions
    {
        public static Point Center(this Rect rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        public static Point TopCenter(this Rect rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Top);
        }

        public static Point BottomCenter(this Rect rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Bottom);
        }

        public static Point LeftCenter(this Rect rect)
        {
            return new Point(rect.Left, rect.Top + rect.Height / 2);
        }

        public static Point RightCenter(this Rect rect)
        {
            return new Point(rect.Right, rect.Top + rect.Height / 2);
        }

        public static Rect ToClientCoordinate(this Rect rect)
        {
            return new Rect(rect.X - SystemParameters.VirtualScreenLeft, rect.Y - SystemParameters.VirtualScreenTop, rect.Width, rect.Height);
        }

        public static Rect ToScreenCoordinate(this Rect rect)
        {
            return new Rect(rect.X + SystemParameters.VirtualScreenLeft, rect.Y + SystemParameters.VirtualScreenTop, rect.Width, rect.Height);
        }
    }
}
