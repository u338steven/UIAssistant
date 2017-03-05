using System;
using System.Collections.Generic;

using UIAssistant.Interfaces.Commands;

namespace UIAssistant.Infrastructure.Commands
{
    public class ArgumentRule : BaseRule, IArgumentRule
    {
        public ArgumentRule(string name, Action<ICommand> action,
            ICollection<IArgumentRule> requiredArgs = null, ICollection<IArgumentRule> optionalArgs = null)
            : base(name, action, requiredArgs, optionalArgs)
        {
        }
    }
}
