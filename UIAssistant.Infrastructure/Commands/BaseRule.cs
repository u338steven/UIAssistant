using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace UIAssistant.Infrastructure.Commands
{
    public class BaseRule : ICandidate
    {
        public static BaseRule Empty = new BaseRule("Empty", new Action<ICommand>(x => { }));

        public string Name { get; private set; }
        public string Description { get; set; }
        public Action<ICommand> Action { get; private set; }

        public ICollection<ArgumentRule> RequiredArgs { get; private set; }
        public ICollection<ArgumentRule> OptionalArgs { get; private set; }

        private BaseRule()
        {

        }

        protected BaseRule(string name, Action<ICommand> action, ICollection<ArgumentRule> requiredArgs = null, ICollection<ArgumentRule> optionalArgs = null)
        {
            Contract.Requires(name != null);
            Contract.Requires(action != null);
            Contract.Ensures(RequiredArgs != null);
            Contract.Ensures(OptionalArgs != null);

            Name = name;
            Action = action;
            RequiredArgs = requiredArgs ?? new List<ArgumentRule>();
            OptionalArgs = optionalArgs ?? new List<ArgumentRule>();
        }
    }
}
