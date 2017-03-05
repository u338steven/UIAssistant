using System;
using System.Collections.Generic;

using UIAssistant.Interfaces.Commands;

namespace UIAssistant.Infrastructure.Commands
{
    public class CommandRule : BaseRule, ICommandRule
    {
        public CommandRule(string command, Action<ICommand> action,
            ICollection<IArgumentRule> requiredArgs = null, ICollection<IArgumentRule> optionalArgs = null)
            : base(command, action, requiredArgs, optionalArgs)
        {
        }
    }
}
