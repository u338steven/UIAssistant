using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Interfaces;
using UIAssistant.Plugin.HitaHint.Enumerators;

namespace UIAssistant.Plugin.HitaHint
{
    class History
    {
        private Stack<HistoryItem> _history = new Stack<HistoryItem>();
        internal bool CanUndo
        {
            get
            {
                return _history.Count > 0;
            }
        }

        internal HistoryItem PopState()
        {
            if (!CanUndo)
            {
                return null;
            }
            return _history.Pop();
        }

        internal void PushState(string operationName, EnumerateTarget target, IWidgetEnumerator enumerator)
        {
            _history.Push(new HistoryItem(operationName, target, enumerator));
        }
    }

    class HistoryItem
    {
        public string OperationName { get; private set; }
        public EnumerateTarget Target { get; private set; }
        public IWidgetEnumerator Enumerator { get; private set; }
        public IWindow window { get; private set; }

        public HistoryItem(string operationName, EnumerateTarget target, IWidgetEnumerator enumerator)
        {
            OperationName = operationName;
            Target = target;
            Enumerator = enumerator;
            window = HitaHint.UIAssistantAPI.ActiveWindow;
        }
    }
}
