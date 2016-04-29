using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.UI.Interfaces;

namespace UIAssistant.Core.Commands
{
    public static class CommandManager
    {
        static CommandStorage _storage = new CommandStorage();

        public static void Add(CommandNode item)
        {
            _storage.Add(item);
        }

        public static bool IsValid(string command)
        {
            return _storage.Validate(command) != null;
        }

        public static ICandidatesGenerator GetGenerator()
        {
            return _storage;
        }
    }
}
