using UIAssistant.Core.Enumerators;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.HitaHint.Operations
{
    class Switch : IOperation
    {
        public bool IsContinuous { get { return false; } }
        public bool IsReturnCursor { get { return true; } }
        public void Dispose() { }
        public void Next(StateController controller) { }
        public void Execute(IHUDItem item)
        {
            (item as WidgetInfo).Window.Activate();
        }
    }
}
