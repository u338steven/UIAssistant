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
using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Core.Themes
{
    public class ThemeSwitcher : ISwitcher
    {
        private ResourceFinder<Theme> _finder;
        private ResourceState<Theme> _state;

        public ThemeSwitcher(string assemblyDir = null)
        {
            var reader = new CacheableResourceReader<Theme>();
            var rootDir = assemblyDir ?? Directory.GetParent(Assembly.GetCallingAssembly().Location).ToString();
            var themeDir = Path.Combine(rootDir, "Themes");
            var resources = new ResourceDirectory<Theme>(new ThemeKeyValueGenerator(), themeDir, "*.xaml");
            _finder = new ResourceFinder<Theme>(resources);
            _state = new ResourceState<Theme>(new ResourceUpdater<Theme>(reader, Application.Current.Resources.MergedDictionaries));
        }

        public void Switch(string id)
        {
            Application.Current.Dispatcher.Invoke(() => _state.Switch(_finder, id));
        }

        public IResourceItem CurrentTheme => _state.Current;

        public void Next()
        {
            Application.Current.Dispatcher.Invoke(() => _state.SwitchNext(_finder));
        }
    }
}
