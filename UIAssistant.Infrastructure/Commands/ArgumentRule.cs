using System;
using System.Collections.Generic;

namespace UIAssistant.Infrastructure.Commands
{
    public class ArgumentRule : BaseRule
    {
        public ArgumentRule(string name, Action<ICommand> action,
            ICollection<ArgumentRule> requiredArgs = null, ICollection<ArgumentRule> optionalArgs = null)
            : base(name, action, requiredArgs, optionalArgs)
        {
        }
    }
}
