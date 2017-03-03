using System;
using System.Collections.Generic;

namespace UIAssistant.Infrastructure.Commands
{
    public class CommandRule : BaseRule
    {
        public CommandRule(string command, Action<ICommand> action,
            ICollection<ArgumentRule> requiredArgs = null, ICollection<ArgumentRule> optionalArgs = null)
            : base(command, action, requiredArgs, optionalArgs)
        {
        }
    }
}
