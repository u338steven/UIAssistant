using System.Windows;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class RunningApp : SearchByTextItem, IWindowItem
    {
        public IWindow Window { get; set; }
        public RunningApp(string fullName, IWindow window)
            : base(fullName, fullName, Rect.Empty, true)
        {
            Window = window;
            SearchByText.UIAssistantAPI.ViewAPI.UIDispatcher.Invoke(() => Image = window.Icon);
        }

        public override void Execute()
        {
            Window.Activate();
        }
    }
}
