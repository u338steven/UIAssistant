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
            controller.State.PreviousWindow = null;
            controller.InvokePlugin("me");
            HitaHint.UIAssistantAPI.DefaultHUD.TextBox.SetText("Mouse Emulation Mode");
        }
        public void Execute(IHUDItem item)
        {
            HitaHint.UIAssistantAPI.MouseAPI.MouseOperation.Move(item.Bounds);
        }
    }
}
