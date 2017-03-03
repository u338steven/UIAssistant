using System.Collections.Generic;

namespace UIAssistant.Infrastructure.Commands
{
    public static class CommandManager
    {
        static CommandSyntax _syntax = new CommandSyntax();
        static IParser _parser = new CommandParser(_syntax);

        public static void Add(CommandRule rule)
        {
            _syntax.Add(rule);
        }

        public static void Clear()
        {
            _syntax.Clear();
        }

        //public static bool IsValid(string command)
        //{
        //    return _storage.Validate(command) != null;
        //}

        public static ICandidatesGenerator GetGenerator()
        {
            return new CandidatesGenerator(_syntax);
        }

        public static IEnumerable<ICommand> Parse(string statement)
        {
            return _parser.Parse(statement);
        }
    }
}
