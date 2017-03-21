using System;

namespace UIAssistant.Interfaces.Session
{
    public interface ISession : IDisposable
    {
        event EventHandler Pausing;
        event EventHandler Resumed;
        event EventHandler Finished;

        void Pause();
        void Resume();
    }
}
