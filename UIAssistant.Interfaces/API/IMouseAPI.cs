using System;
using UIAssistant.Interfaces.Input;
using UIAssistant.Interfaces.Session;

namespace UIAssistant.Interfaces.API
{
    public interface IMouseAPI
    {
        IMouseCursor MouseCursor { get; }
        IMouseOperation MouseOperation { get; }

        void ReserveToReturnMouseCursor(ISession session, Func<bool> canReturn);
    }
}