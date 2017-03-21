using System;
using System.Threading.Tasks;
using UIAssistant.Core.Input;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Session;

namespace UIAssistant.Core.API
{
    class MouseAPI : IMouseAPI
    {
        public IMouseCursor MouseCursor { get; } = new MouseCursor();
        public IMouseOperation MouseOperation { get; } = new MouseOperation();

        public void ReserveToReturnMouseCursor(ISession session, Func<bool> canReturn)
        {
            var prevMousePosition = MouseOperation.GetMousePosition();
            session.Finished += (_, __) =>
            {
                if (!canReturn())
                {
                    return;
                }
                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(300);
                    MouseOperation.Move(prevMousePosition);
                });
            };
        }
    }
}
