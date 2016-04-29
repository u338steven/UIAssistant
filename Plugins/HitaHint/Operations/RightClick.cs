using UIAssistant.Core.Enumerators;
using UIAssistant.Core.Input;

namespace UIAssistant.Plugin.HitaHint.Operations
{
    class RightClick : IOperation
    {
        public bool IsContinuous { get { return false; } }
        public bool IsReturnCursor { get { return true; } }
        public void Dispose() { }
        public void Next(StateController controller) { }
        public void Execute(IHUDItem item)
        {
            MouseOperation.RightClick(item.Bounds);
        }
    }
}
