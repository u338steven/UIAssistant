using System;

using UIAssistant.Interfaces.Commands;

namespace UIAssistant.Infrastructure.Commands
{
    public class Command : ICommand
    {
        public string Name { get; }
        public string Value { get; }
        public Action<ICommand> Action { get; }

        public Command(string name, string value = null, Action<ICommand> action = null)
        {
            Name = name;
            Value = value;
            Action = action;
        }

        public void Invoke()
        {
            Action?.Invoke(this);
        }
    }
}
