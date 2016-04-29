using UIAssistant.Core.Enumerators;
using UIAssistant.Core.Input;

namespace UIAssistant.Plugin.HitaHint.Operations
{
    class Click : IOperation
    {
        public bool IsContinuous { get { return false; } }
        public bool IsReturnCursor { get { return true; } }
        public void Dispose() { }
        public void Next(StateController controller) { }
        public void Execute(IHUDItem item)
        {
            MouseOperation.Click(item.Bounds);
        }
    }
}
