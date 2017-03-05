using UIAssistant.Interfaces.HUD;
using UIAssistant.Plugin.HitaHint.Enumerators;

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
