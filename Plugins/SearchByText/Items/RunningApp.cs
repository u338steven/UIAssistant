using System.Windows;
using UIAssistant.Utility.Win32;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class RunningApp : SearchByTextItem
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
