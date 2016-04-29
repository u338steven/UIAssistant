using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;

namespace UIAssistant.Core.Themes
{
    public class ThemeSwitcher
    {
        Themes _themes = new Themes(Directory.GetParent(Assembly.GetCallingAssembly().Location).ToString());
        public void Switch(string name)
        {
            var theme = _themes.Find(name);
            if (theme == null)
            {
                theme = _themes.Find(_themes.Default);
            }
            _themes.Switch(theme);
        }

        public Theme CurrentTheme => _themes.Current;

        public Theme Find(string name)
        {
            return _themes.Find(name);
        }

        public IList<Theme> AvailableThemes
        {
            get
            {
                return _themes.GetAvailables();
            }
        }

        public void Next()
        {
            _themes.Next();
        }
    }
}
