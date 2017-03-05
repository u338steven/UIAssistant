using System.Collections.Generic;

using UIAssistant.Interfaces.Commands;
using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Infrastructure.Commands
{
    public static class CommandManager
    {
        static CommandSyntax _syntax = new CommandSyntax();
        static IParser _parser = new CommandParser(_syntax);

        public static void Add(ICommandRule rule)
        {
            _syntax.Add(rule);
        }

        public static void Clear()
        {
            _syntax.Clear();
        }

        public static ICandidatesGenerator GetGenerator()
        {
            return new CandidatesGenerator(_syntax);
        }

        public static IValidatable<string> GetValidator(ILocalizer localizer)
        {
            return new CommandValidator(_parser, localizer);
        }

        public static IEnumerable<ICommand> Parse(string statement)
        {
            return _parser.Parse(statement);
        }
    }
}
