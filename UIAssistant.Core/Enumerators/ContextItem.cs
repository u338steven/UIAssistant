using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using UIAssistant.Core.API;
using UIAssistant.Core.I18n;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Core.Enumerators
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
            DisplayText = TextID.ActionsCopy.GetLocalizedText();
            IsEnabled = true;
        }

        public override void Execute()
        {
            var item = UIAssistantAPI.Instance.DefaultHUD.SelectedItem;
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
            DisplayText = TextID.ActionsCopyAll.GetLocalizedText();
            IsEnabled = true;
        }

        public override void Execute()
        {
            var result = UIAssistantAPI.Instance.DefaultHUD.Items.Select(x => x.DisplayText).Aggregate((x, y) => $"{x}\r\n{y}");
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(result));
        }
    }

    public class CopyHwnd : ContextItemBase
    {
        public CopyHwnd()
        {
            DisplayText = TextID.ActionsCopyHwnd.GetLocalizedText();
            IsEnabled = true;
        }

        public override void Execute()
        {
            var item = UIAssistantAPI.Instance.DefaultHUD.SelectedItem as IWindowItem;
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
            DisplayText = TextID.ActionsToggleTopMost.GetLocalizedText();
            IsEnabled = true;
        }

        public override void Execute()
        {
            var item = UIAssistantAPI.Instance.DefaultHUD.SelectedItem as IWindowItem;
            item?.Window.ToggleTopMost();
        }
    }

    public class CloseWindow : ContextItemBase
    {
        public CloseWindow()
        {
            DisplayText = TextID.ActionsCloseWindow.GetLocalizedText();
            IsEnabled = true;
        }

        public override void Execute()
        {
            var item = UIAssistantAPI.Instance.DefaultHUD.SelectedItem as IWindowItem;
            item?.Window.Close();
        }
    }
}
