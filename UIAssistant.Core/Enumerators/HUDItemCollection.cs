using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

namespace UIAssistant.Core.Enumerators
{
    public class HUDItemCollection : ObservableCollection<IHUDItem>
    {
        private object _lock;
        public HUDItemCollection()
        {
            Initialize();
        }

        public HUDItemCollection(IEnumerable<IHUDItem> collection) : base(collection)
        {
            Initialize();
        }

        private void Initialize()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _lock = new object();
                System.Windows.Data.BindingOperations.EnableCollectionSynchronization(this, _lock);
            });
        }
    }
}
