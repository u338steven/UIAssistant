using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;

using UIAssistant.Interfaces;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Core.Enumerators
{
    public class WidgetInfo : IHUDItem, IWindowItem
    {
        public string InternalText { get; set; }
        public string DisplayText { get; set; }
        public Point Location { get; set; }
        public Rect Bounds { get; set; }
        public ImageSource Image { get { return Window?.Icon; } }
        public IWindow Window { get; set; }
        public int ColoredStart { get; set; }
        public int ColoredLength { get; set; }

        public WidgetInfo(string text, Rect bounds)
        {
            DisplayText = text;
            Bounds = bounds;
            Location = bounds.Location;
        }

        public WidgetInfo(Rect bounds)
        {
            Bounds = bounds;
            Location = bounds.Location;
        }

        public virtual void Execute()
        {
        }
    }
}
