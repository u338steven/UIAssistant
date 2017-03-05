using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Core.Themes
{
    public static class ThemeDefaultSwitcher
    {
        static ThemeSwitcher _theme = new ThemeSwitcher();
        public static void Switch(string name)
        {
            _theme.Switch(name);
        }

        public static IResourceItem CurrentTheme => _theme.CurrentTheme;

        public static void Next()
        {
            _theme.Next();
        }
    }
}
