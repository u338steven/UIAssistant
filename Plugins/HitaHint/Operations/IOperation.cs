using System;
using UIAssistant.Core.Enumerators;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.HitaHint.Operations
{
    interface IOperation : IDisposable
    {
        void Execute(IHUDItem item);
        bool IsReturnCursor { get; }
        bool IsContinuous { get; }
        void Next(StateController controller);
    }
}
