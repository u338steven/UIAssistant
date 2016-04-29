using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Automation;
using System.Text.RegularExpressions;

using UIAssistant.Core.Enumerators;
using UIAssistant.Utility.Win32;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.SearchByText.Enumerators.ForCommand
{
    abstract class AbstarctSearchForCommand : IDisposable
    {
        public event Action Updated;
        public event Action Finished;
        protected IntPtr MainWindowHandle { get; private set; } = Win32Window.ActiveWindow.WindowHandle;
        protected HUDItemCollection _results;

        public virtual void Dispose()
        {
            Updated = null;
            Finished = null;
        }

        public abstract void Enumerate(HUDItemCollection items);

        public void Update()
        {
            Updated?.Invoke();
        }

        public void Finish()
        {
            Finished?.Invoke();
            Updated = null;
            Finished = null;
        }

        public static bool FormatName(string parent, ControlType elementType, AutomationElement el, ref string addName, out string fullpath, out string shortcutKey, out bool canExpand)
        {
            fullpath = "";
            shortcutKey = "";
            canExpand = false;

            if (parent != null)
            {
                if (ControlType.List != elementType)
                {
                    addName = parent + Consts.Delimiter + addName;
                }
                else
                {
                    addName = parent;
                }
            }
            addName = Regex.Replace(addName, @"[\r\n\t]", "");

            if (!elementType.IsThroughElement())
            {
                fullpath = addName;
                canExpand = el.CanExpandElement(elementType);
                if (canExpand)
                {
                    fullpath = addName + Consts.Expandable;
                }
                shortcutKey = el.GetShortcutKey();
                return true;
            }
            return false;
        }
    }
}
