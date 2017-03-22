using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using UIAssistant.Core.I18n;
using UIAssistant.Infrastructure.Commands;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Commands;

namespace UIAssistant.Core.API
{
    class CommandAPI : ICommandAPI
    {
        public void RegisterCommand(ICommandRule rule)
        {
            CommandManager.Add(rule);
        }

        public IEnumerable<ICommand> ParseStatement(string statement)
        {
            return CommandManager.Parse(statement);
        }

        public ICandidatesGenerator GetCommandGenerator()
        {
            return CommandManager.GetGenerator();
        }

        public ValidationResult Validate(string statement)
        {
            return CommandManager.GetValidator(DefaultLocalizer.Instance).Validate(statement);
        }

        public ICommandRule CreateCommandRule(string name, Action<ICommand> action, ICollection<IArgumentRule> requiredArgs = null, ICollection<IArgumentRule> optionalArgs = null)
        {
            return new CommandRule(name, action, requiredArgs, optionalArgs);
        }

        public IArgumentRule CreateArgmentRule(string name, Action<ICommand> action, ICollection<IArgumentRule> requiredArgs = null, ICollection<IArgumentRule> optionalArgs = null)
        {
            return new ArgumentRule(name, action, requiredArgs, optionalArgs);
        }

        public void InvokePluginCommand(string command, Action quit = null, Action pausing = null, Action resumed = null)
        {
            if (UIAssistantAPI.Instance.PluginManager.Exists(command))
            {
                pausing?.Invoke();
                UIAssistantAPI.Instance.DefaultHUD.Initialize();
                UIAssistantAPI.Instance.PluginManager.Execute(command);
                UIAssistantAPI.Instance.PluginManager.Resume += () =>
                {
                    resumed?.Invoke();
                };
                UIAssistantAPI.Instance.PluginManager.Quit += () =>
                {
                    quit?.Invoke();
                };
            }
            else
            {
                UIAssistantAPI.Instance.NotificationAPI.NotifyWarnMessage("Plugin Error", string.Format(TextID.CommandNotFound.GetLocalizedText(), command));
                quit?.Invoke();
            }
        }
    }
}
