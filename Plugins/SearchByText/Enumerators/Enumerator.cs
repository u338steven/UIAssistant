using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Enumerators;

namespace UIAssistant.Plugin.SearchByText.Enumerators
{
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
                default:
                    return new SearchForText();
            }
        }
    }
}
