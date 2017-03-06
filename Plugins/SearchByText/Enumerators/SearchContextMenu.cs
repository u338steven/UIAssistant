using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using UIAssistant.Interfaces.HUD;
using UIAssistant.Plugin.SearchByText.Enumerators.ForCommand;

namespace UIAssistant.Plugin.SearchByText.Enumerators
{
    class SearchContextMenu : ISearchByTextEnumerator
    {
        const int MN_GETHMENU = 0x01E1;
        public event EventHandler Updated;
        public event EventHandler Finished;
        AbstarctSearchForCommand _current;

        public void Enumerate(ICollection<IHUDItem> results)
        {
            var menuEnumerator = new ContextMenu();
            var observer = SearchByText.UIAssistantAPI.GetObserver(Interfaces.Events.ObserberKinds.PopupObserver);
            observer.Callback += (element) =>
            {
                menuEnumerator.ContextRoot = element;
                SearchByText.UIAssistantAPI.TopMost = true;
            };

            observer.Observe();

            SearchByText.UIAssistantAPI.KeyboardOperation.PressedKeyUp();
            SearchByText.UIAssistantAPI.KeyboardOperation.SendKeys(Key.Apps);
            if (!observer.Wait(2000))
            {
                // timeout
                observer.Dispose();
                Finished?.Invoke(this, EventArgs.Empty);
                Finished = null;
                return;
            }
            observer.Dispose();

            EnumerateInternal(menuEnumerator, results);
            Finished?.Invoke(this, EventArgs.Empty);
            Finished = null;
        }

        private void EnumerateInternal(AbstarctSearchForCommand enumerator, ICollection<IHUDItem> results)
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
