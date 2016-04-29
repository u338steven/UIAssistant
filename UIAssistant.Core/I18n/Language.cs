using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Resource;

namespace UIAssistant.Core.I18n
{
    public class Language : IResourceItem
    {
        public string CultureName { get; set; }
        public string DisplayName { get; set; }

        public Language(string cultureName, string displayName)
        {
            CultureName = cultureName;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public string FileName { get { return CultureName; } }
    }
}
