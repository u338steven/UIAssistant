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
                System.Windows.Data.BindingOperations.EnableCollectionSynchronization(this, new object());
            });
        }
    }
}
