using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using UIAssistant.Core.API;
using UIAssistant.Core.Tools;
using UIAssistant.Interfaces.HUD;

namespace UIAssistant.Core.Enumerators
{
    public class DefaultFilter : IFilter
    {
        public DefaultFilter()
        {
            var api = UIAssistantAPI.Instance;
            try
            {
                if (!Migemo.IsEnable() && api.UIAssistantSettings.UseMigemo)
                {
                    Migemo.Initialize(api.UIAssistantSettings.MigemoDllPath, api.UIAssistantSettings.MigemoDictionaryPath);
                }
            }
            catch (Exception ex)
            {
                api.NotificationAPI.NotifyWarnMessage("Load Migemo Error", $"{ex.Message}");
                api.LogAPI.WriteErrorMessage(ex);
            }
        }

        public IEnumerable<IHUDItem> Filter(IEnumerable<IHUDItem> list, string input)
        {
            return Filter(list, input.Split(' '));
        }

        private IEnumerable<IHUDItem> Filter(IEnumerable<IHUDItem> list, params string[] inputs)
        {
            Regex regex;
            if (CanUseMigemo(inputs[0]))
            {
                regex = Migemo.GetRegex(inputs[0]);
            }
            else
            {
                var input = Regex.Escape(inputs[0]);
                regex = new Regex(input, RegexOptions.IgnoreCase);
            }
            var select = list.Where(hudItem =>
            {
                var match = regex.Match(hudItem.DisplayText);
                if (match.Success)
                {
                    if (hudItem == null)
                    {
                        return false;
                    }
                    hudItem.ColoredStart = match.Index;
                    hudItem.ColoredLength = match.Length;
                }
                return match.Success;
            });
            if (inputs.Length > 1)
            {
                return Filter(select, inputs.Skip(1).ToArray());
            }
            else
            {
                return select;
            }
        }

        private readonly Regex _ascii = new Regex("^[\x20-\x7E]+$");
        private bool CanUseMigemo(string input)
        {
            if (input.Length > 1 && Migemo.IsEnable() && UIAssistantAPI.Instance.UIAssistantSettings.UseMigemo)
            {
                return _ascii.IsMatch(input);
            }
            return false;
        }
    }
}
