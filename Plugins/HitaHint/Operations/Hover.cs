using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.HitaHint.Operations
{
    class Hover : IOperation
    {
        public bool IsContinuous { get { return false; } }
        public bool IsReturnCursor { get { return false; } }
        public void Dispose() { }
        public void Next(StateController controller) { }
        public void Execute(IHUDItem item)
        {
            HitaHint.UIAssistantAPI.MouseAPI.MouseOperation.Move(item.Bounds);
        }
    }
}
