using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using UIAssistant.Core.Enumerators;
using KeybindHelper.LowLevel;

namespace UIAssistant.Plugin
{
    public interface IHUD
    {
        ITextBox TextBox { get; set; }
        HUDItemCollection Items { get; set; }
        IHUDItem SelectedItem { get; }
        int ItemsCountPerPage { get; set; }
        Func<IEnumerable<IHUDItem>, string, IEnumerable<IHUDItem>> CustomFilter { get; set; }
        void Initialize();
        void Update();
        void FocusNextItem();
        void FocusPreviousItem();
        void PageDown();
        void PageUp();
        void Filter(HUDItemCollection list, string input);
        void Execute();
    }
}
