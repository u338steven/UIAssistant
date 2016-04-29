using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.HitaHint.Operations
{
    public static class OperationManager
    {
        public static string CurrentName { get; private set; }
        internal static IOperation CurrentCommand { get; private set; }

        private static Dictionary<string, IOperation> _actions = new Dictionary<string, IOperation>();

        static OperationManager()
        {
            _actions.Add(Consts.Click, new Click());
            _actions.Add(Consts.RightClick, new RightClick());
            _actions.Add(Consts.MiddleClick, new MiddleClick());
            _actions.Add(Consts.DoubleClick, new DoubleClick());
            _actions.Add(Consts.Hover, new Hover());
            _actions.Add(Consts.DragAndDrop, new DragAndDrop());
            _actions.Add(Consts.Dragged, new Dragged());
            _actions.Add(Consts.Drop, new Drop());
            _actions.Add(Consts.Switch, new Switch());
            _actions.Add(Consts.MouseEmulation, new MouseEmulation());
        }

        public static void SetDefaultOpration(IList<string> args)
        {
            CurrentName = Consts.Click;
            CurrentCommand = _actions[CurrentName];
            _actions.ForEach(pair =>
            {
                if (args.Contains(pair.Key, StringComparer.CurrentCultureIgnoreCase))
                {
                    CurrentName = pair.Key;
                    CurrentCommand = pair.Value;
                    return;
                }
            });
        }

        public static void Change(string operationName)
        {
            if (_actions.ContainsKey(operationName))
            {
                CurrentName = operationName;
                CurrentCommand = _actions[operationName];
            }
        }
    }
}
