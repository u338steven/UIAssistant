using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Enumerators;

namespace UIAssistant.Plugin.HitaHint.Enumerators
{
    public interface IWidgetEnumerator : IDisposable
    {
        void Enumerate(HUDItemCollection container);
    }
}
