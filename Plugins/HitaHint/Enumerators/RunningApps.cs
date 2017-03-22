using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.HitaHint.Enumerators
{
    class RunningApps : IWidgetEnumerator
    {
        public void Enumerate(ICollection<IHUDItem> container)
        {
            var desktopBounds = HitaHint.UIAssistantAPI.Screen.Bounds;
            var results = new List<IHUDItem>();

            HitaHint.UIAssistantAPI.WindowAPI.EnumerateWindows((window) =>
            {
                if (window.IsAltTabWindow())
                {
                    return true;
                }

                var rect = window.Bounds.ToClientCoordinate();
                WidgetInfo item = new WidgetInfo(": " + window.Title, rect);
                item.Window = window;
                HitaHint.UIAssistantAPI.ViewAPI.UIDispatcher.Invoke(() => AdjustLocation(results, item, desktopBounds));
                results.Add(item);
                return true;
            });
            // 重なり解消のため必要
            results.OrderBy(item => item.Bounds.X).ToList().ForEach(x => container.Add(x));
        }

        private static void AdjustLocation(IList<IHUDItem> results, IHUDItem item, System.Windows.Rect desktopBounds)
        {
            var x = item.Location.X;
            var y = item.Location.Y;
            var adjustedY = y;
            var paddingY = 20d;
            var paddingX = 6d;

            if (item.Image != null)
            {
                paddingY += Math.Min(item.Image.Height, 32);
                paddingX += Math.Min(item.Image.Width, 32);
            }

            var sorted = results.OrderBy(i => i.Location.Y);
            foreach (var i in sorted)
            {
                if (i.Location.Y - paddingY <= adjustedY && i.Location.Y + paddingY >= adjustedY)
                {
                    if (i.Location.X - paddingX <= x && i.Location.X + paddingX >= x)
                    {
                        adjustedY = i.Location.Y + paddingY;
                    }
                }
            }

            if (adjustedY + paddingY > desktopBounds.Height)
            {
                adjustedY = desktopBounds.Height - paddingY;
            }

            item.Location = new System.Windows.Point(item.Location.X, adjustedY);
        }

        public void Dispose()
        {
        }
    }
}
