using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Enumerators;
using UIAssistant.Utility;

namespace UIAssistant.Plugin.HitaHint.Enumerators
{
    class DividedScreen : IWidgetEnumerator
    {
        public void Enumerate(HUDItemCollection container)
        {
            var divisionCount = HitaHintSettings.Instance.ScreenWidthDivisionCount;
            foreach (var screen in Screen.AllScreen)
            {
                Divide(container, screen, divisionCount);
            }
        }

        const int _divideMinPixel = 16;
        private static void Divide(HUDItemCollection container, System.Windows.Rect screen, int divisionCount)
        {
            var desktopBounds = Screen.Bounds;
            var relativeRectangle = new System.Windows.Rect(0, 0, 0, 0);

            relativeRectangle.Width = screen.Width / divisionCount;
            if (screen.Width % divisionCount > 0)
            {
                ++relativeRectangle.Width;
            }
            if (relativeRectangle.Width < _divideMinPixel)
            {
                divisionCount = (int)screen.Width / _divideMinPixel;
                relativeRectangle.Width = screen.Width / divisionCount;
            }
            relativeRectangle.Height = relativeRectangle.Width;

            int heightCount = (int)(screen.Height / relativeRectangle.Height);
            double heightRemainder = screen.Height % relativeRectangle.Height;
            if (heightRemainder > _divideMinPixel)
            {
                ++heightCount;
            }
            else if (heightRemainder > 0)
            {
                relativeRectangle.Height += 1 + heightRemainder / divisionCount;
                if (screen.Height - relativeRectangle.Height * heightCount > heightCount)
                {
                    ++heightCount;
                }
            }
            double width = relativeRectangle.Width;
            double height = relativeRectangle.Height;
            for (int i = 0; i < divisionCount; ++i)
            {
                relativeRectangle.Width = width;
                relativeRectangle.X = i * width + screen.X - desktopBounds.Left;
                if (relativeRectangle.Right + desktopBounds.Left > screen.Right)
                {
                    relativeRectangle.Width -= relativeRectangle.Right + desktopBounds.Left - screen.Right + 1;
                }
                for (int j = 0; j < heightCount; ++j)
                {
                    relativeRectangle.Height = height;
                    relativeRectangle.Y = j * height + screen.Y - desktopBounds.Top;
                    if (relativeRectangle.Bottom + desktopBounds.Top > screen.Bottom)
                    {
                        relativeRectangle.Height -= relativeRectangle.Bottom + desktopBounds.Top - screen.Bottom + 1;
                    }
                    container.Add(new WidgetInfo(relativeRectangle));
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
