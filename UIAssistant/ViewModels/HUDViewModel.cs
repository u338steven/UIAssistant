using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Livet;

using UIAssistant.Core.API;
using UIAssistant.Core.Enumerators;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Utility;
using KeybindHelper.LowLevel;

namespace UIAssistant.ViewModels
{
    public class HUDViewModel : ViewModel, IHUD
    {
        #region Items変更通知プロパティ
        private ICollection<IHUDItem> _Items;
        public ICollection<IHUDItem> Items
        {
            get
            { return _Items; }
            set
            {
                if (_Items == value)
                    return;
                _Items = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(BoundsList));
            }
        }

        #endregion

        #region SelectedIndex変更通知プロパティ
        private int _SelectedIndex;

        public int SelectedIndex
        {
            get
            { return _SelectedIndex; }
            set
            {
                if (_SelectedIndex == value)
                    return;
                _SelectedIndex = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region ItemsCountPerPage変更通知プロパティ
        private int _ItemsCountPerPage;

        public int ItemsCountPerPage
        {
            get
            { return _ItemsCountPerPage; }
            set
            {
                if (_ItemsCountPerPage == value)
                    return;
                _ItemsCountPerPage = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region TextBox変更通知プロパティ
        private ITextBox _TextBox;

        public ITextBox TextBox
        {
            get
            { return _TextBox; }
            set
            {
                if (_TextBox == value)
                    return;
                _TextBox = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public CoordinateOrigin CoordinateOrigin { get; private set; } = new CoordinateOrigin();

        public IHUDItem SelectedItem
        {
            get
            {
                if (Items.Count == 1)
                {
                    return Items.ElementAt(0);
                }
                return Items.ElementAtOrDefault(SelectedIndex);
            }
        }

        public IEnumerable<Rect> BoundsList { get { return Items.Select(item => item.Bounds); } }

        public HUDViewModel()
        {
            SelectedIndex = -1;
            Items = new HUDItemCollection();
            TextBox = new LowLevelTextBoxViewModel();
            CustomFilter = new DefaultFilter();
        }

        public void Initialize()
        {
            SelectedIndex = -1;
            DispatcherHelper.UIDispatcher.Invoke(() => Items = new HUDItemCollection());
            TextBox.Initialize();
            CustomFilter = new DefaultFilter();
            CoordinateOrigin = new CoordinateOrigin();
            RaisePropertyChanged(nameof(CoordinateOrigin));
            ItemsCountPerPage = UIAssistantAPI.Instance.UIAssistantSettings.ItemsCountPerPage;
        }

        public void Update()
        {
            RaisePropertyChanged(nameof(BoundsList));
            Items = new HUDItemCollection(Items.OrderBy(x => x.Bounds.X));
        }

        public void FocusNextItem()
        {
            if (SelectedIndex >= Items.Count - 1)
            {
                SelectedIndex = 0;
                return;
            }
            ++SelectedIndex;
        }

        public void FocusPreviousItem()
        {
            if (SelectedIndex <= 0)
            {
                SelectedIndex = Items.Count - 1;
                return;
            }
            --SelectedIndex;
        }

        public void PageDown()
        {
            if (Items.Count <= SelectedIndex + ItemsCountPerPage)
            {
                SelectedIndex = Items.Count - 1;
                return;
            }
            SelectedIndex += ItemsCountPerPage;
        }

        public void PageUp()
        {
            if (SelectedIndex - ItemsCountPerPage < 0)
            {
                SelectedIndex = 0;
                return;
            }
            SelectedIndex -= ItemsCountPerPage;
        }

        public void Execute()
        {
            SelectedItem?.Execute();
        }

        public IFilter CustomFilter { get; set; }
        public void Filter(ICollection<IHUDItem> items, string input)
        {
            var clonedItems = new HUDItemCollection(items);
            Items = new HUDItemCollection(CustomFilter.Filter(clonedItems, input));
        }
    }
}
