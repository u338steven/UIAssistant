using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using UIAssistant.Core.Events;
using UIAssistant.Core.Input;
using UIAssistant.Core.Enumerators;
using UIAssistant.Plugin.SearchByText.Enumerators.ForCommand;

namespace UIAssistant.Plugin.SearchByText.Enumerators
{
    class SearchContextMenu : ISearchByTextEnumerator
    {
        const int MN_GETHMENU = 0x01E1;
        public event Action Updated;
        public event Action Finished;
        AbstarctSearchForCommand _current;

        public void Enumerate(HUDItemCollection results)
        {
            var menuEnumerator = new ContextMenu();
            var observer = new PopupObserver();
            observer.Callback += (element) =>
            {
                menuEnumerator.ContextRoot = element;
                UIAssistantAPI.TopMost = true;
            };

            observer.Observe();

            KeyboardOperation.PressedKeyUp();
            KeyboardOperation.SendKeys(Key.Apps);
            if (!observer.Wait(2000))
            {
                // timeout
                observer.Dispose();
                Finished?.Invoke();
                Finished = null;
                return;
            }
            observer.Dispose();

            EnumerateInternal(menuEnumerator, results);
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
            _current?.Dispose();
            Updated = null;
            Finished = null;
        }
    }
}
