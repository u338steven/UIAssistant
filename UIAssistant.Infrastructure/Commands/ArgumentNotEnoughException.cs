using System;

namespace UIAssistant.Infrastructure.Commands
{
    internal class ArgumentNotEnoughException : ArgumentException
    {
        internal ArgumentNotEnoughException(string message) : base(message)
        {

        }
    }
}
