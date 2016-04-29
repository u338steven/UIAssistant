using System;
using UIAssistant.Core.Enumerators;

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
