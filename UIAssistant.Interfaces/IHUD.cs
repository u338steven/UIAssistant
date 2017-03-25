using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Automation;
using System.Windows.Media;
using KeybindHelper.LowLevel;

namespace UIAssistant.Interfaces.HUD
{
    public interface IHUD
    {
        ITextBox TextBox { get; }
        ICollection<IHUDItem> Items { get; set; }
        IHUDItem SelectedItem { get; }
        int ItemsCountPerPage { get; set; }
        IFilter CustomFilter { get; set; }
        void Initialize();
        void Update();
        void FocusNextItem();
        void FocusPreviousItem();
        void PageDown();
        void PageUp();
        void Filter(ICollection<IHUDItem> list, string input);
        void Execute();
    }

    public interface IHUDItem
    {
        string InternalText { get; set; }
        string DisplayText { get; set; }
        Point Location { get; set; }
        Rect Bounds { get; set; }
        ImageSource Image { get; }
        int ColoredStart { get; set; }
        int ColoredLength { get; set; }

        void Execute();
    }

    public interface IWindowItem
    {
        IWindow Window { get; set; }
    }

    public interface IHUDItemEnumerator
    {
        void Enumerate(ICollection<IHUDItem> container, System.Windows.Automation.Condition condition, params ControlType[] types);
        void Retry(ICollection<IHUDItem> container);
    }

    public interface IFilter
    {
        IEnumerable<IHUDItem> Filter(IEnumerable<IHUDItem> list, string input);
    }
}
