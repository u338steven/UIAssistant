using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using UIAssistant.Interfaces.Commands;

namespace UIAssistant.Infrastructure.Commands
{
    public class BaseRule : ICandidate, IRule
    {
        public static IRule Empty = new BaseRule("Empty", new Action<ICommand>(x => { }));

        public string Name { get; private set; }
        public string Description { get; set; }
        public Action<ICommand> Action { get; private set; }

        public ICollection<IArgumentRule> RequiredArgs { get; private set; }
        public ICollection<IArgumentRule> OptionalArgs { get; private set; }

        private BaseRule()
        {

        }

        protected BaseRule(string name, Action<ICommand> action, ICollection<IArgumentRule> requiredArgs = null, ICollection<IArgumentRule> optionalArgs = null)
        {
            Contract.Requires(name != null);
            Contract.Requires(action != null);
            Contract.Ensures(RequiredArgs != null);
            Contract.Ensures(OptionalArgs != null);

            Name = name;
            Action = action;
            RequiredArgs = requiredArgs ?? new List<IArgumentRule>();
            OptionalArgs = optionalArgs ?? new List<IArgumentRule>();
        }
    }
}
