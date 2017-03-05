using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.HitaHint.Operations
{
    class MiddleClick : IOperation
    {
        public bool IsContinuous { get { return false; } }
        public bool IsReturnCursor { get { return true; } }
        public void Dispose() { }
        public void Next(StateController controller) { }
        public void Execute(IHUDItem item)
        {
            HitaHint.UIAssistantAPI.MouseOperation.MiddleClick(item.Bounds);
        }
    }
}
