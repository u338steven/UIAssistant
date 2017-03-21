using UIAssistant.Interfaces.HUD;
using UIAssistant.Plugin.HitaHint.Enumerators;

namespace UIAssistant.Plugin.HitaHint.Operations
{
    class DragAndDrop : IOperation
    {
        public bool IsContinuous { get { return true; } }
        public bool IsReturnCursor { get { return true; } }
        public void Dispose() { }
        public void Next(StateController controller)
        {
            if (controller.State.Target == EnumerateTarget.DividedScreen)
            {
                controller.SaveState();
                controller.ChangeOperation(Consts.Drop);
            }
            else if (controller.State.Target != EnumerateTarget.DividedScreen)
            {
                controller.SaveState();
                controller.ChangeOperation(Consts.Dragged);
                controller.ChangeTarget(EnumerateTarget.RunningApps);
            }
            controller.Clear();
            controller.Enumerate();
            controller.PrintState();
        }
        public void Execute(IHUDItem item)
        {
            HitaHint.UIAssistantAPI.MouseOperation.Drag(item.Bounds);
        }
    }

    class Dragged : IOperation
    {
        public bool IsContinuous { get { return true; } }
        public bool IsReturnCursor { get { return true; } }
        public void Dispose()
        {
            HitaHint.UIAssistantAPI.KeyboardOperation.SendKeys(System.Windows.Input.Key.Escape);
            HitaHint.UIAssistantAPI.MouseOperation.LeftUp();
        }
        public void Next(StateController controller)
        {
            controller.SaveState();
            controller.ChangeOperation(Consts.Drop);
            controller.ChangeTarget(EnumerateTarget.WidgetsInWindow);
            controller.Clear();
            controller.Enumerate();
            controller.PrintState();
        }
        public void Execute(IHUDItem item)
        {
            (item as WidgetInfo).Window.Activate();
        }
    }

    class Drop : IOperation
    {
        public bool IsContinuous { get { return false; } }
        public bool IsReturnCursor { get { return true; } }
        public void Dispose()
        {
            HitaHint.UIAssistantAPI.KeyboardOperation.SendKeys(System.Windows.Input.Key.Escape);
            HitaHint.UIAssistantAPI.MouseOperation.LeftUp();
        }
        public void Next(StateController controller) { }
        public void Execute(IHUDItem item)
        {
            HitaHint.UIAssistantAPI.MouseOperation.Drop(item.Bounds);
        }
    }
}
