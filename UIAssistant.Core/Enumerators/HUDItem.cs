using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Automation;
using System.Windows.Media;

using UIAssistant.Utility.Win32;

namespace UIAssistant.Core.Enumerators
{
    public interface IHUDItemEnumerator
    {
        void Enumerate(HUDItemCollection container, System.Windows.Automation.Condition condition, params ControlType[] types);
        void Enumerate(HUDItemCollection container, Win32Window root, System.Windows.Automation.Condition condition, params ControlType[] types);
        void Retry(HUDItemCollection container);
    }

    public enum EnumerateTarget
    {
        WidgetsInWindow,
        RunningApps,
        WidgetsInTaskbar,
        DividedScreen,
        Commands,
        TextsInWindow,
        TextsInContainer,
    }

    public interface IHUDItem
    {
        string InternalText { get; set; }
        string DisplayText { get; set; }
        Point Location { get; set; }
        Rect Bounds { get; set; }
        ImageSource Image { get; }
        int ColoredStart { get; set; }
        int ColoredLength { get; set; }

        void Execute();
    }

    public class WidgetInfo : IHUDItem
    {
        public string InternalText { get; set; }
        public string DisplayText { get; set; }
        public Point Location { get; set; }
        public Rect Bounds { get; set; }
        public ImageSource Image { get { return Window?.Icon; }}
        public Win32Window Window { get; set; }
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
