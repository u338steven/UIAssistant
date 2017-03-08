using System;

namespace UIAssistant.Interfaces.Session
{
    public interface ISession : IDisposable
    {
        event EventHandler Paused;
        event EventHandler Resumed;
        event EventHandler Finished;

        void Pause();
        void Resume();
    }
}
