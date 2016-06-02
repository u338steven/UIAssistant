using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Markup;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    public class CommandTypeExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(typeof(CommandType));
            return enumValues.Cast<CommandType>().Select(x => new EnumerationMember { Value = x, Description = GetLocalizedText(x) }).ToArray();
        }

        private string GetLocalizedText(CommandType value)
        {
            var localizer = KeybindsManiacs.Localizer;

            switch (value)
            {
                case CommandType.SwapKey:
                    return localizer.GetLocalizedText("kmSwapKey");
                case CommandType.RunEmbeddedCommand:
                    return localizer.GetLocalizedText("kmRunEmbeddedCommand");
                case CommandType.RunUIAssistantCommand:
                    return localizer.GetLocalizedText("kmRunUIAssistantCommand");
                //case CommandType.RunExtensionCommand:
                //    return _localizer.GetLocalizedText("kmRunExtensionCommand");
                default:
                    break;
            }
            return "";
        }

        public class EnumerationMember
        {
            public string Description { get; set; }
            public object Value { get; set; }
        }
    }
}
