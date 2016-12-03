using System.Windows;
using UIAssistant.Core.Enumerators;
using UIAssistant.Utility.Win32;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class RunningApp : SearchByTextItem, IWindowItem
    {
        public Win32Window Window { get; set; }
        public RunningApp(string fullName, Win32Window window)
            : base(fullName, fullName, Rect.Empty, true)
        {
            Window = window;
            UIAssistantAPI.UIDispatcher.Invoke(() => Image = window.Icon);
        }

        public override void Execute()
        {
            Window.Activate();
        }
    }
}
