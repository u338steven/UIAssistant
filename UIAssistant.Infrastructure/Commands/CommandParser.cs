using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace UIAssistant.Infrastructure.Commands
{
    public class CommandParser : IParser
    {
        private ICommandSyntax _syntax;

        public CommandParser(ICommandSyntax syntax)
        {
            Contract.Requires(syntax != null);
            _syntax = syntax;
        }

        public IEnumerable<ICommand> Parse(string statement)
        {
            BaseRule command = null;
            BaseRule current = null;
            List<ArgumentRule> options = new List<ArgumentRule>();

            var tokens = statement.Tokenize();

            var commandWord = tokens.ElementAt(0);
            if (!_syntax.ContainsWord(commandWord))
            {
                throw new ArgumentException(commandWord);
            }
            command = _syntax.FindRule(commandWord);
            current = command;
            options.AddRange(command.OptionalArgs);
            yield return new Command(commandWord, action: command.Action);

            foreach(var word in tokens.Skip(1))
            {
                if (current.RequiredArgs.ContainsWord(word))
                {
                    current = current.RequiredArgs.FindRule(word);
                    options.AddRange(current.OptionalArgs);
                    yield return new Command(word, action: current.Action);
                    continue;
                }

                var wordSplited = word.SplitIntoKeyValue();
                var option = options.FindRule(wordSplited.Key);
                if (!option.IsNullOrEmpty())
                {
                    yield return new Command(wordSplited.Key, wordSplited.Value, option.Action);
                    continue;
                }

                throw new ArgumentException(word);
            }
        }
    }
}
