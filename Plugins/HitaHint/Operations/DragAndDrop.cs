using UIAssistant.Core.Enumerators;
using UIAssistant.Core.Input;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.HitaHint.Operations
{
    class DragAndDrop : IOperation
    {
        public bool IsContinuous { get { return true; } }
        public bool IsReturnCursor { get { return true; } }
        public void Dispose() { }
        public void Next(StateController controller)
        {
            if (controller.Target == EnumerateTarget.DividedScreen)
            {
                controller.SaveState();
                controller.ChangeOperation(Consts.Drop);
            }
            else if (controller.Target != EnumerateTarget.DividedScreen)
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
            MouseOperation.Drag(item.Bounds);
        }
    }

    class Dragged : IOperation
    {
        public bool IsContinuous { get { return true; } }
        public bool IsReturnCursor { get { return true; } }
        public void Dispose()
        {
            KeyboardOperation.SendKeys(System.Windows.Input.Key.Escape);
            MouseOperation.LeftUp();
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
            KeyboardOperation.SendKeys(System.Windows.Input.Key.Escape);
            MouseOperation.LeftUp();
        }
        public void Next(StateController controller) { }
        public void Execute(IHUDItem item)
        {
            MouseOperation.Drop(item.Bounds);
        }
    }
}
