using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Settings;

namespace UIAssistant.Models
{
    public class PluginTabModel
    {
        public bool Enabled { get; set; }
        public ImageSource Icon { get; set; }
        public IPluginMetadata Metadata { get; set; }
        public FrameworkElement SettingsPanel { get; set; }
    }

    class PluginContextLoader
    {
        Dictionary<int, PluginTabModel> _cache = new Dictionary<int, PluginTabModel>();
        IUserSettings _settings;

        public PluginContextLoader(IUserSettings settings)
        {
            _settings = settings;
        }

        public PluginTabModel LoadPluginViewContext(IEnumerable<Lazy<IPlugin, IPluginMetadata>> plugins, int selectedIndex)
        {
            if (_cache.ContainsKey(selectedIndex))
            {
                return _cache[selectedIndex];
            }

            var context = new PluginTabModel();
            var plugin = plugins.ElementAt(selectedIndex);
            context.Metadata = plugin.Metadata;
            context.Enabled = !_settings.DisabledPlugins.Contains(context.Metadata.Guid);
            context.SettingsPanel = (plugin.Value as IConfigurablePlugin)?.GetConfigrationInterface();

            var uri = new Uri(plugin.Metadata.IconUri, UriKind.RelativeOrAbsolute);
            if (IsLocalFile(uri))
            {
                context.Icon = new BitmapImage(uri);
            }
            _cache.Add(selectedIndex, context);
            return context;
        }

        private bool IsLocalFile(Uri uri)
        {
            if (uri.IsAbsoluteUri && !uri.IsLoopback)
            {
                return false;
            }
            return true;
        }

        public void DisablePlugin(PluginTabModel context)
        {
            _settings.DisabledPlugins.Add(context.Metadata.Guid);
        }

        public void EnablePlugin(PluginTabModel context)
        {
            _settings.DisabledPlugins.Remove(context.Metadata.Guid);
        }
    }
}
