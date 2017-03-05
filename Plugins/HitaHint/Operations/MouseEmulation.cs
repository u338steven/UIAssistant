using UIAssistant.Core.Input;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.HitaHint.Operations
{
    class MouseEmulation : IOperation
    {
        public bool IsContinuous { get { return true; } }
        public bool IsReturnCursor { get { return true; } }
        public void Dispose() { }
        public void Next(StateController controller)
        {
            controller.PreviousWindow = null;
            controller.InvokePlugin("me");
            UIAssistantAPI.DefaultHUD.TextBox.SetText("Mouse Emulation Mode");
        }
        public void Execute(IHUDItem item)
        {
            MouseOperation.Move(item.Bounds);
        }
    }
}
