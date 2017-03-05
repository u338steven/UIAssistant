using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Livet;

using System.Windows;
using System.Text.RegularExpressions;
using UIAssistant.Core.Enumerators;
using UIAssistant.Core.Tools;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Plugin;
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
            ItemsCountPerPage = 8;
            Items = new HUDItemCollection();
            TextBox = new LowLevelTextBoxViewModel();
            try
            {
                if (!Migemo.IsEnable() && UIAssistantAPI.UIAssistantSettings.UseMigemo)
                {
                    Migemo.Initialize(UIAssistantAPI.UIAssistantSettings.MigemoDllPath, UIAssistantAPI.UIAssistantSettings.MigemoDictionaryPath);
                }
            }
            catch (Exception ex)
            {
                UIAssistantAPI.NotifyWarnMessage("Load Migemo Error", $"{ex.Message}");
            }
        }

        public void Initialize()
        {
            SelectedIndex = -1;
            DispatcherHelper.UIDispatcher.Invoke(() => Items = new HUDItemCollection());
            TextBox.Initialize();
            CustomFilter = null;
            CoordinateOrigin = new CoordinateOrigin();
            RaisePropertyChanged(nameof(CoordinateOrigin));
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

        public Func<IEnumerable<IHUDItem>, string, IEnumerable<IHUDItem>> CustomFilter { get; set; }
        public void Filter(ICollection<IHUDItem> items, string input)
        {
            HUDItemCollection clonedItems = new HUDItemCollection(items);
            if (CustomFilter != null)
            {
                Items = new HUDItemCollection(CustomFilter.Invoke(clonedItems, input));
                return;
            }
            Items = new HUDItemCollection(DefaultFilter(clonedItems, input.Split(' ')));
        }

        private Regex _ascii = new Regex("^[\x20-\x7E]+$");
        private bool UseMigemo(string input)
        {
            if (input.Length > 1 && Migemo.IsEnable() && UIAssistantAPI.UIAssistantSettings.UseMigemo)
            {
                return _ascii.IsMatch(input);
            }
            return false;
        }

        private IEnumerable<IHUDItem> DefaultFilter(IEnumerable<IHUDItem> list, params string[] inputs)
        {
            Regex regex;
            if (UseMigemo(inputs[0]))
            {
                regex = Migemo.GetRegex(inputs[0]);
            }
            else
            {
                var input = Regex.Escape(inputs[0]);
                regex = new Regex(input, RegexOptions.IgnoreCase);
            }
            var select = list.Where(hudItem =>
            {
                var match = regex.Match(hudItem.DisplayText);
                if (match.Success)
                {
                    if (hudItem == null)
                    {
                        return false;
                    }
                    hudItem.ColoredStart = match.Index;
                    hudItem.ColoredLength = match.Length;
                }
                return match.Success;
            });
            if (inputs.Length > 1)
            {
                return DefaultFilter(select, inputs.Skip(1).ToArray());
            }
            else
            {
                return select;
            }
        }
    }
}
