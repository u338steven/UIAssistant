using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Resource;

namespace UIAssistant.Core.Themes
{
    public class Theme : IResourceItem
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public Theme(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string FileName { get { return Name; } }
    }
}
