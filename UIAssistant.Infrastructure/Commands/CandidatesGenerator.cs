using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using UIAssistant.Interfaces.Commands;

namespace UIAssistant.Infrastructure.Commands
{
    public class CandidatesGenerator : ICandidatesGenerator
    {
        private ICommandSyntax _syntax;
        private static IReadOnlyCollection<ICandidate> Empty = new List<ICandidate>();

        public CandidatesGenerator(ICommandSyntax syntax)
        {
            Contract.Requires(syntax != null);
            _syntax = syntax;
        }

        public IEnumerable<ICandidate> GenerateCandidates(string sentence)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ICandidate>>() != null);

            if (sentence == null)
            {
                return Empty;
            }

            var commandTokens = sentence.Tokenize();
            var command = commandTokens.ElementAt(0);
            var args = commandTokens.Skip(1);

            if (_syntax.ContainsWord(command))
            {
                return GetArgumentCandidates(command, args).OrderBy(x => x.Name);
            }
            else
            {
                if (commandTokens.Count() > 1)
                {
                    return Empty;
                }
                return GetCommandCandidates(command).OrderBy(x => x.Name);
            }
        }

        private IEnumerable<ICandidate> GetCommandCandidates(string command)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ICandidate>>() != null);

            if (command.Length > 0)
            {
                return _syntax.GetCandidates(command);
            }
            return _syntax;
        }

        private IEnumerable<ICandidate> GetArgumentCandidates(string command, IEnumerable<string> args)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ICandidate>>() != null);

            if (args.Count() == 0)
            {
                return Empty;
            }

            var rule = _syntax.FindRule(command);

            var currentWord = args.Last();
            var currentRules = rule.FindApplyingRules(args);
            var lastRule = currentRules.Last();

            if (currentWord.IsOption())
            {
                var options = currentRules.Where(x => x.OptionalArgs != null).SelectMany(x => x.OptionalArgs);
                if (currentWord.HasOptionValue())
                {
                    var keyValue = currentWord.SplitIntoKeyValue();
                    return options.FindRule(keyValue.Key).OptionalArgs.GetCandidates(keyValue.Value);
                }
                return options.GetCandidates(currentWord);
            }

            return lastRule.RequiredArgs.GetCandidates(currentWord);
        }
    }
}
