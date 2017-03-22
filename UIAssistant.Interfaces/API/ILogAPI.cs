using System;

namespace UIAssistant.Interfaces.API
{
    public interface ILogAPI
    {
        void WriteDebugMessage(string message);
        void WriteErrorMessage(Exception ex, string message = null);
    }
}