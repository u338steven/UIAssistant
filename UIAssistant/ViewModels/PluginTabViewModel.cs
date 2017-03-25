using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using UIAssistant.Models;
using UIAssistant.Core.API;
using UIAssistant.Interfaces.Plugin;

namespace UIAssistant.ViewModels
{
    public class PluginTabViewModel : ViewModel
    {
        #region Plugins変更通知プロパティ
        private IEnumerable<Lazy<IPlugin, IPluginMetadata>> _Plugins;

        public IEnumerable<Lazy<IPlugin, IPluginMetadata>> Plugins
        {
            get
            { return _Plugins; }
            set
            {
                if (_Plugins == value)
                    return;
                _Plugins = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public PluginTabModel Context { get; set; }
        PluginContextLoader _loader;

        public void Initialize()
        {
            _loader = new PluginContextLoader(UIAssistantAPI.Instance.UIAssistantSettings);
            Plugins = UIAssistantAPI.Instance.PluginManager.Plugins;
            LoadPluginView(0);
        }

        public void OnChecked()
        {
            _loader.EnablePlugin(Context);
        }
        
        public void OnUnchecked()
        {
            _loader.DisablePlugin(Context);
        }

        public void LoadPluginView(int selectedIndex)
        {
            if (Plugins.Count() <= selectedIndex)
            {
                return;
            }

            Context = _loader.LoadPluginViewContext(Plugins, selectedIndex);
            RaisePropertyChanged(nameof(Context));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
