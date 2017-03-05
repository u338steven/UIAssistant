using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.SearchByText.Items
{
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

        public abstract void Execute();
    }

    public class Copy : ContextItemBase
    {
        public Copy()
        {
            DisplayText = SearchByText.UIAssistantAPI.Localize(TextID.ActionsCopy);
            IsEnabled = true;
        }

        public override void Execute()
        {
            var item = SearchByText.UIAssistantAPI.DefaultHUD.SelectedItem;
            if (item == null)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(item.InternalText));
        }
    }

    public class CopyAll : ContextItemBase
    {
        public CopyAll()
        {
            DisplayText = SearchByText.UIAssistantAPI.Localize(TextID.ActionsCopyAll);
            IsEnabled = true;
        }

        public override void Execute()
        {
            var result = SearchByText.UIAssistantAPI.DefaultHUD.Items.Select(x => x.DisplayText).Aggregate((x, y) => $"{x}\r\n{y}");
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(result));
        }
    }

    public class CopyHwnd : ContextItemBase
    {
        public CopyHwnd()
        {
            DisplayText = SearchByText.UIAssistantAPI.Localize(TextID.ActionsCopyHwnd);
            IsEnabled = true;
        }

        public override void Execute()
        {
            var item = SearchByText.UIAssistantAPI.DefaultHUD.SelectedItem as IWindowItem;
            if (item == null)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(item.Window.WindowHandle.ToString()));
        }
    }

    public class ToggleTopMost : ContextItemBase
    {
        public ToggleTopMost()
        {
            DisplayText = SearchByText.UIAssistantAPI.Localize(TextID.ActionsToggleTopMost);
            IsEnabled = true;
        }

        public override void Execute()
        {
            var item = SearchByText.UIAssistantAPI.DefaultHUD.SelectedItem as IWindowItem;
            item?.Window.ToggleTopMost();
        }
    }

    public class CloseWindow : ContextItemBase
    {
        public CloseWindow()
        {
            DisplayText = SearchByText.UIAssistantAPI.Localize(TextID.ActionsCloseWindow);
            IsEnabled = true;
        }

        public override void Execute()
        {
            var item = SearchByText.UIAssistantAPI.DefaultHUD.SelectedItem as IWindowItem;
            item?.Window.Close();
        }
    }
}
