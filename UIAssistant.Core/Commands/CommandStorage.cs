using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using UIAssistant.Utility.Extensions;
using UIAssistant.Core.I18n;
using UIAssistant.UI.Interfaces;

namespace UIAssistant.Core.Commands
{
    public class CommandStorage : List<CommandNode>, ICandidatesGenerator
    {
        public ValidationResult Validate(string commandSource)
        {
            var commandTokens = Tokenize(commandSource);

            if (this.Any(cmdNode => cmdNode.IsResponsibleFor(commandTokens) &&
                (cmdNode.Arguments == null || cmdNode.Arguments.Any(argNode => argNode.Equals(commandTokens.Where(token => !token.StartsWithCaseIgnored("-")).Skip(1))))))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(TextID.InvalidCommand.GetLocalizedText());
        }

        private bool IsValidOptions(IEnumerable<string> commandTokens)
        {
            var options = this.Find(cmdNode => cmdNode.IsResponsibleFor(commandTokens)).Options;
            return commandTokens.Where(token => token.StartsWithCaseIgnored("-")).Any(token => options.Any(option => token.EqualsWithCaseIgnored(option.Value)));
        }

        public IEnumerable<string> GenerateCandidates(string commandSource)
        {
            var commandTokens = Tokenize(commandSource);
            if (IsCommandFixed(commandTokens))
            {
                return GetArgumentCandidates(commandTokens)?.OrderBy(s => s);
            }
            else
            {
                if (commandTokens.Count() > 1)
                {
                    return null;
                }
                return GetCommandCandidates(commandTokens)?.OrderBy(s => s);
            }
        }

        public bool IsCommandFixed(IEnumerable<string> commandTokens)
        {
            if (commandTokens.Count() < 1)
            {
                return false;
            }
            return this.Any(cmdNode => cmdNode.IsResponsibleFor(commandTokens));
        }

        private IEnumerable<string> GetCommandCandidates(IEnumerable<string> commandTokens)
        {
            if (commandTokens.Count() > 0)
            {
                return this.Where(cmdNode => cmdNode.Value.StartsWithCaseIgnored(commandTokens.ElementAt(0))).Select(cmdNode => cmdNode.Value);
            }
            return this.Select(cmdNode => cmdNode.Value);
        }

        private IEnumerable<string> GetArgumentCandidates(IEnumerable<string> commandTokens)
        {
            var currentCommandNode = this.Find(cmdNode => cmdNode.IsResponsibleFor(commandTokens));
            var arguments = currentCommandNode.Arguments;
            commandTokens = commandTokens.Skip(1);

            commandTokens = commandTokens.SkipWhile(token =>
            {
                var nextNode = arguments?.ToList().Find(argNode => argNode.Value.EqualsWithCaseIgnored(token));
                if (nextNode == null)
                {
                    return false;
                }
                arguments = nextNode?.Arguments;
                return true;
            });

            if (commandTokens.Count() != 1)
            {
                return null;
            }
            return arguments?.Where(argNode => argNode.Value.StartsWithCaseIgnored(commandTokens.Last()))?.Select(argNode => argNode.Value);
        }

        public IEnumerable<string> Tokenize(string commandSource)
        {
            return commandSource.ToLower().Split(' ');
        }
    }
}

