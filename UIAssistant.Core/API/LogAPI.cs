using System;
using UIAssistant.Infrastructure.Logger;
using UIAssistant.Interfaces.API;

namespace UIAssistant.Core.API
{
    class LogAPI : ILogAPI
    {
        public void WriteDebugMessage(string message)
        {
            Log.Debug(message);
        }

        public void WriteErrorMessage(Exception ex, string message = null)
        {
            Log.Error(ex, message);
        }
    }
}
