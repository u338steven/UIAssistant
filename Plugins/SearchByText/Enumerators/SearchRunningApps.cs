using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Enumerators;
using UIAssistant.Utility.Extensions;
using UIAssistant.Utility.Win32;
using UIAssistant.Plugin.SearchByText.Items;

namespace UIAssistant.Plugin.SearchByText.Enumerators
{
    class SearchRunningApps : ISearchByTextEnumerator
    {
#pragma warning disable 414
        public event Action Updated;
        public event Action Finished;
#pragma warning restore 414
        public void Enumerate(HUDItemCollection resultsContainer)
        {
            var results = new List<IHUDItem>();
            Win32Window.Filter((window) =>
            {
                if (window.IsAltTabWindow())
                {
                    return true;
                }

                var item = new RunningApp(window.Title, window);
                results.Add(item);
                return true;
            });
            results.OrderBy(x => x.DisplayText).ForEach(x => resultsContainer.Add(x));
        }

        public void Cancel()
        {
            Dispose();
        }

        public void Dispose()
        {
            Updated = null;
            Finished = null;
        }
    }
}
