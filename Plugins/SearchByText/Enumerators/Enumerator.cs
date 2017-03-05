using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIAssistant.Plugin.SearchByText.Enumerators
{
    public enum EnumerateTarget
    {
        RunningApps,
        Commands,
        TextsInWindow,
        TextsInContainer,
        ContextMenu,
    }

    class Enumerator
    {
        public static ISearchByTextEnumerator Factory(EnumerateTarget target)
        {
            switch (target)
            {
                case EnumerateTarget.Commands:
                    return new SearchForCommand();
                case EnumerateTarget.TextsInWindow:
                    return new SearchForText();
                case EnumerateTarget.TextsInContainer:
                    return new SearchContainer();
                case EnumerateTarget.RunningApps:
                    return new SearchRunningApps();
                case EnumerateTarget.ContextMenu:
                    return new SearchContextMenu();
                default:
                    return new SearchForText();
            }
        }
    }
}
