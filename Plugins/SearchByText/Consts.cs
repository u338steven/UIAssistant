using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIAssistant.Plugin.SearchByText
{
    internal static class Consts
    {
        internal const string PluginName = "Search by Text";
        internal const string Delimiter = " | ";
        internal const string Expandable = " > ";

        internal const string Command = "/";

        internal const string AutoFire = "-" + nameof(AutoFire);

        internal const string Commands = nameof(Commands);
        internal const string TextsInWindow = nameof(TextsInWindow);
        internal const string TextsInContainer = nameof(TextsInContainer);
        internal const string RunningApps = nameof(RunningApps);
        internal const string ContextMenu = nameof(ContextMenu);
    }
}
