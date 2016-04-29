using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Enumerators;

namespace UIAssistant.Plugin.HitaHint.Enumerators
{
    class Enumerator
    {
        public static IWidgetEnumerator Factory(EnumerateTarget target)
        {
            switch (target)
            {
                case EnumerateTarget.WidgetsInWindow:
                    return new WidgetsInWindow();
                case EnumerateTarget.RunningApps:
                    return new RunningApps();
                case EnumerateTarget.WidgetsInTaskbar:
                    return new WidgetsInTaskbar();
                case EnumerateTarget.DividedScreen:
                    return new DividedScreen();
                default:
                    return new WidgetsInWindow();
            }
        }
    }
}
