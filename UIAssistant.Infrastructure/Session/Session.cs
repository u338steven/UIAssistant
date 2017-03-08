using System;

using UIAssistant.Interfaces.Session;

namespace UIAssistant.Infrastructure.Session
{
    public class Session : ISession
    {
        public event EventHandler Finished;
        public event EventHandler Paused;
        public event EventHandler Resumed;

        public void Pause()
        {
            Paused?.Invoke(this, EventArgs.Empty);
        }

        public void Resume()
        {
            Resumed?.Invoke(this, EventArgs.Empty);
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            Finished?.Invoke(this, EventArgs.Empty);
            Finished = null;
            Paused = null;
            Resumed = null;
        }
    }
}
