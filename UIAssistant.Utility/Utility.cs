using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace UIAssistant.Utility
{
    public static class Utility
    {
    }

    public sealed class TimeMeasurement : IDisposable
    {
        private System.Diagnostics.Stopwatch _stopwatch;
        public TimeMeasurement()
        {
            System.Diagnostics.Debug.WriteLine($"Start {DateTime.Now.ToShortTimeString()}");
            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine($"Stop  {DateTime.Now.ToShortTimeString()} ({_stopwatch.ElapsedMilliseconds} ms)");
        }
    }
}
