using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIAssistant.Core.Themes
{
    public static class ThemeDefaultSwitcher
    {
        static ThemeSwitcher _theme = new ThemeSwitcher();
        public static void Switch(string name)
        {
            _theme.Switch(name);
        }

        public static Theme CurrentTheme => _theme.CurrentTheme;

        public static Theme Find(string name)
        {
            return _theme.Find(name);
        }

        public static IList<Theme> AvailableThemes
        {
            get
            {
                return _theme.AvailableThemes;
            }
        }

        public static void Next()
        {
            _theme.Next();
        }
    }
}
