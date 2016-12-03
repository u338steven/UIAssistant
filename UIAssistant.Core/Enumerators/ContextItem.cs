using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using UIAssistant.Utility.Win32;
using UIAssistant.Core.I18n;

namespace UIAssistant.Core.Enumerators
{
    public interface IWindowItem
    {
        Win32Window Window { get; set; }
    }

    public abstract class ContextItemBase : IHUDItem
    {
        public bool IsEnabled { get; set; }
        public string InternalText { get; set; }
        public string DisplayText { get; set; }
        public Point Location { get; set; }
        public Rect Bounds { get; set; }
        public ImageSource Image { get; set; }
        public int ColoredStart { get; set; }
        public int ColoredLength { get; set; }

        public void Execute()
        {
        }

        public abstract void Execute(IHUDItem item);
    }

    public class Copy : ContextItemBase
    {
        public Copy()
        {
            DisplayText = TextID.ActionsCopy.GetLocalizedText();
            IsEnabled = true;
        }

        public override void Execute(IHUDItem item)
        {
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(item.InternalText));
        }
    }

    public class CopyHwnd : ContextItemBase
    {
        public CopyHwnd()
        {
            DisplayText = TextID.ActionsCopyHwnd.GetLocalizedText();
            IsEnabled = true;
        }

        public override void Execute(IHUDItem item)
        {
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText((item as IWindowItem).Window.WindowHandle.ToString()));
        }
    }

    public class ToggleTopMost : ContextItemBase
    {
        public ToggleTopMost()
        {
            DisplayText = TextID.ActionsToggleTopMost.GetLocalizedText();
            IsEnabled = true;
        }

        public override void Execute(IHUDItem item)
        {
            (item as IWindowItem)?.Window.ToggleTopMost();
        }
    }

    public class CloseWindow : ContextItemBase
    {
        public CloseWindow()
        {
            DisplayText = TextID.ActionsCloseWindow.GetLocalizedText();
            IsEnabled = true;
        }

        public override void Execute(IHUDItem item)
        {
            (item as IWindowItem)?.Window.Close();
        }
    }
}
