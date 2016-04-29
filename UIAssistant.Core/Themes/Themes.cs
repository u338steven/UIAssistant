using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using UIAssistant.Core.Resource;

namespace UIAssistant.Core.Themes
{
    internal class Themes : Resource<Theme>
    {
        public override string Default => "General";
        private string _directoryPath;
        protected override string DirectoryPath => _directoryPath;

        public Themes(string rootDirectory)
        {
            _directoryPath = Path.Combine(rootDirectory, "Themes");
            Current = Find(Default);
            Switch(Current);
        }

        protected override void CreateAvailableDictionaries(string[] files)
        {
            AvailableDictionaries = files.Select(c => new Theme(c, Path.Combine(DirectoryPath, c))).ToList();
        }
    }
}
