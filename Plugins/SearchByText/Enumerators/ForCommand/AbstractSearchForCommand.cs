using System;
using System.Collections.Generic;

using System.Windows.Automation;
using System.Text.RegularExpressions;

using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Plugin.SearchByText.Enumerators.ForCommand
{
    abstract class AbstarctSearchForCommand : IDisposable
    {
        public event EventHandler Updated;
        public event EventHandler Finished;
        protected IntPtr MainWindowHandle { get; private set; } = SearchByText.UIAssistantAPI.WindowAPI.ActiveWindow.WindowHandle;
        protected ICollection<IHUDItem> _results;

        public virtual void Dispose()
        {
            Updated = null;
            Finished = null;
        }

        public abstract void Enumerate(ICollection<IHUDItem> items);

        public void Update()
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }

        public void Finish()
        {
            Finished?.Invoke(this, EventArgs.Empty);
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
