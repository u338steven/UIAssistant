using System;
using System.Collections.Generic;

using UIAssistant.Interfaces.Commands;

namespace UIAssistant.Infrastructure.Commands
{
    public class CommandSyntax : List<ICommandRule>, ICommandSyntax
    {
    }
}

