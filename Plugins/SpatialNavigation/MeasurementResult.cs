using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Automation;

namespace UIAssistant.Plugin.SpatialNavigation
{
    class MeasurementResult
    {
        public AutomationElement Element { get; private set; }
        public double Distance { get; private set; }

        public MeasurementResult(AutomationElement element, double distance)
        {
            Element = element;
            Distance = distance;
        }
    }
}
