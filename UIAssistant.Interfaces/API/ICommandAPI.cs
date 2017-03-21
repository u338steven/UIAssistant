using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UIAssistant.Interfaces.Commands;

namespace UIAssistant.Interfaces.API
{
    public interface ICommandAPI
    {
        IArgumentRule CreateArgmentRule(string name, Action<ICommand> action, ICollection<IArgumentRule> requiredArgs = null, ICollection<IArgumentRule> optionalArgs = null);
        ICommandRule CreateCommandRule(string name, Action<ICommand> action, ICollection<IArgumentRule> requiredArgs = null, ICollection<IArgumentRule> optionalArgs = null);
        ICandidatesGenerator GetCommandGenerator();
        void InvokePluginCommand(string command, Action quit = null, Action pausing = null, Action resumed = null);
        IEnumerable<ICommand> ParseStatement(string statement);
        void RegisterCommand(ICommandRule rule);
        ValidationResult Validate(string statement);
    }
}
