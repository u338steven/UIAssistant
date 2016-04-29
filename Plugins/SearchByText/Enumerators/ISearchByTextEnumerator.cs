using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Enumerators;

namespace UIAssistant.Plugin.SearchByText.Enumerators
{
    internal interface ISearchByTextEnumerator : IDisposable
    {
        void Enumerate(HUDItemCollection resultsContainer);
        void Cancel();
        event Action Updated;
        event Action Finished;
    }
}
