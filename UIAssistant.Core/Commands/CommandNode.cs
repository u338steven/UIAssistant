using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIAssistant.Core.Commands
{
    public class CommandNode : CommandBaseNode
    {
        public IEnumerable<OptionNode> Options;

        public CommandNode(string command, IEnumerable<ArgumentNode> arguments = null, IEnumerable<OptionNode> options = null)
            : base(command, arguments)
        {
            Options = options;
        }
    }
}
