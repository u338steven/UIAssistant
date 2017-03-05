using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.HitaHint.Enumerators
{
    public interface IWidgetEnumerator : IDisposable
    {
        void Enumerate(ICollection<IHUDItem> container);
    }
}
