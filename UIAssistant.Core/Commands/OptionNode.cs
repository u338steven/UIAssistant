using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIAssistant.Core.Commands
{
    public class OptionNode : CommandBaseNode
    {
        public OptionNode(string option, params string[] arguments)
            : base(option)
        {
            Arguments = arguments.Select(arg => new ArgumentNode(arg));
        }

        public OptionNode(string option, IEnumerable<ArgumentNode> arguments = null)
            : base(option, arguments)
        {
        }
    }
}
