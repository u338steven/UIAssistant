﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using UIAssistant.Interfaces.Commands;

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
            if (statement == null)
            {
                throw new ArgumentException("");
            }

            IRule command = null;
            IRule current = null;
            List<IArgumentRule> options = new List<IArgumentRule>();

            var tokens = statement.Tokenize();

            var commandWord = tokens.ElementAt(0);
            if (!_syntax.ContainsWord(commandWord))
            {
                throw new ArgumentException(commandWord);
            }
            command = _syntax.FindRule(commandWord);
            current = command;
            options.AddRange(command.OptionalArgs);

            if (!IsEnoughArguments(current, commandWord, tokens))
            {
                throw new ArgumentNotEnoughException(commandWord);
            }
            yield return new Command(commandWord, action: command.Action);

            foreach(var word in tokens.Skip(1))
            {
                if (current.RequiredArgs.ContainsWord(word))
                {
                    current = current.RequiredArgs.FindRule(word);
                    options.AddRange(current.OptionalArgs);
                    if (!IsEnoughArguments(current, word, tokens))
                    {
                        throw new ArgumentNotEnoughException(word);
                    }
                    yield return new Command(word, action: current.Action);
                    continue;
                }

                var wordSplited = word.SplitIntoKeyValue();
                var option = options.FindRule(wordSplited.Key);
                if (!option.IsNullOrEmpty())
                {
                    if (!IsEnoughArguments(current, word, tokens))
                    {
                        throw new ArgumentNotEnoughException(current.Name);
                    }
                    yield return new Command(wordSplited.Key, wordSplited.Value, option.Action);
                    continue;
                }

                throw new ArgumentException(word);
            }
        }

        private bool IsEnoughArguments(IRule rule, string currentWord, IEnumerable<string> tokens)
        {
            return rule.RequiredArgs.Count == 0 || tokens.Last() != currentWord;
        }
    }
}
