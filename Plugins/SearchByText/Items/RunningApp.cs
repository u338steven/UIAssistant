using System.Windows;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Utility.Win32;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class RunningApp : SearchByTextItem, IWindowItem
    {
        public IWindow Window { get; set; }
        public RunningApp(string fullName, Win32Window window)
            : base(fullName, fullName, Rect.Empty, true)
        {
            Window = window;
            SearchByText.UIAssistantAPI.UIDispatcher.Invoke(() => Image = window.Icon);
        }

        public override void Execute()
        {
            Window.Activate();
        }
    }
}
