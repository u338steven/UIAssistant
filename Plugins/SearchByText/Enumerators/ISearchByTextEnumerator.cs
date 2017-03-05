using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.SearchByText.Enumerators
{
    internal interface ISearchByTextEnumerator : IDisposable
    {
        void Enumerate(ICollection<IHUDItem> resultsContainer);
        void Cancel();
        event EventHandler Updated;
        event EventHandler Finished;
    }
}
