using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;
using System.Windows;

using UIAssistant.Infrastructure.Resource;
using UIAssistant.Infrastructure.Resource.Theme;

namespace UIAssistant.Core.Themes
{
    public class ThemeSwitcher
    {
        private ResourceFinder<Theme> _finder;
        private ResourceState<Theme> _state;

        public ThemeSwitcher()
        {
            var reader = new CacheableResourceReader<Theme>();
            var dirPath = Path.Combine(Directory.GetParent(Assembly.GetCallingAssembly().Location).ToString(), "Themes");
            var resources = new ResourceDirectory<Theme>(new ThemeKeyValueGenerator(), dirPath, "*.xaml");
            _finder = new ResourceFinder<Theme>(resources);
            _state = new ResourceState<Theme>(new ResourceUpdater<Theme>(reader, Application.Current.Resources.MergedDictionaries));
        }

        public void Switch(string id)
        {
            Application.Current.Dispatcher.Invoke(() => _state.Switch(_finder, id));
        }

        public Theme CurrentTheme => _state.Current;

        public void Next()
        {
            Application.Current.Dispatcher.Invoke(() => _state.SwitchNext(_finder));
        }
    }
}
