using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Utility.Extensions;

namespace UIAssistant.Core.Commands
{
    public class CommandBaseNode
    {
        public string Value;
        public IEnumerable<ArgumentNode> Arguments;

        public CommandBaseNode(string command, IEnumerable<ArgumentNode> arguments = null)
        {
            Value = command;
            Arguments = arguments;
        }

        public bool IsResponsibleFor(IEnumerable<string> commandTokens)
        {
            if (commandTokens.Count() == 0)
            {
                return false;
            }
            return Value.EqualsWithCaseIgnored(commandTokens.ElementAt(0));
        }
    }

}
