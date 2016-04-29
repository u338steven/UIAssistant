using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Enumerators;
using UIAssistant.Plugin.SearchByText.Enumerators.ForCommand;

namespace UIAssistant.Plugin.SearchByText.Enumerators
{
    class SearchForCommand : ISearchByTextEnumerator
    {
        public event Action Updated;
        public event Action Finished;
        AbstarctSearchForCommand _current;

        public void Enumerate(HUDItemCollection results)
        {
            EnumerateInternal(new RibbonUI(), results);
            if (results.Count > 0)
            {
                Finished?.Invoke();
                return;
            }

            EnumerateInternal(new Menu(), results);
            if (results.Count == 0)
            {
                EnumerateInternal(new WPFMenu(), results);
            }
            EnumerateInternal(new Toolbar(), results);
            Finished?.Invoke();
            Finished = null;
        }

        private void EnumerateInternal(AbstarctSearchForCommand enumerator, HUDItemCollection results)
        {
            _current = enumerator;
            _current.Updated += Updated;
            _current.Enumerate(results);
            _current.Updated -= Updated;
        }

        public void Cancel()
        {
            Dispose();
        }

        public void Dispose()
        {
            _current.Dispose();
            Updated = null;
            Finished = null;
        }
    }
}
