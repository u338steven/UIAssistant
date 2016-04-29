using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Utility.Extensions;

namespace UIAssistant.Core.Commands
{
    public class ArgumentNode : CommandBaseNode
    {
        public ArgumentNode(string argument, params string[] arguments)
            : base(argument)
        {
            Arguments = arguments.Select(arg => new ArgumentNode(arg));
        }

        public ArgumentNode(string argument, IEnumerable<ArgumentNode> subArguments = null)
            : base(argument, subArguments)
        {
            Arguments = subArguments;
        }

        public bool Equals(IEnumerable<string> args)
        {
            if (args == null || args.Count() == 0)
            {
                if (!string.IsNullOrEmpty(Value))
                {
                    return false;
                }
                return true;
            }
            if (Value.EqualsWithCaseIgnored(args.ElementAt(0)))
            {
                if (Arguments == null)
                {
                    if (args.Count() == 1)
                    {
                        return true;
                    }
                    return false;
                }
                return Arguments.Any(arg => arg.Equals(args?.Skip(1)));
            }
            return false;
        }
    }
}
