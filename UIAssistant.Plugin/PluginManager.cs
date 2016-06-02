using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using UIAssistant.Utility.Extensions;
using UIAssistant.Core.Settings;
using UIAssistant.Core.I18n;
using UIAssistant.Core.Logger;

namespace UIAssistant.Plugin
{
    public class PluginManager : IDisposable
    {
        public static PluginManager Instance { get; } = new PluginManager();
        private CompositionContainer container;
        private Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();

        private string directoryPath => UIAssistantDirectory.Plugins;

        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        public IEnumerable<Lazy<IPlugin, IPluginMetadata>> Plugins { get; set; }

        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        private IEnumerable<ILocalizablePlugin> LocalizablePlugins { get; set; }

        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        private IEnumerable<IDisposable> DisposablePlugins { get; set; }

        private PluginManager() { Initialize(); }

        private void Initialize()
        {
            var catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            var pluginPaths = Directory.EnumerateFiles(directoryPath, "*.dll", SearchOption.AllDirectories);

            pluginPaths.ForEach(pluginPath =>
            {
                var asmCatalog = new AssemblyCatalog(pluginPath);
                if (asmCatalog.Parts.ToList().Count > 0)
                {
                    catalog.Catalogs.Add(asmCatalog);
                }
            });

            container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            Plugins.ForEach(plugin =>
            {
                try
                {
                    plugin.Value.Initialize();
                }
                catch(Exception ex)
                {
                    var message = string.Format(TextID.PluginInitializeError.GetLocalizedText(), plugin.Metadata.Name);
                    UIAssistantAPI.NotifyWarnMessage("Warning", message);
                    Log.Error(ex);
                    Log.Warn(message);
                }
                if (!plugins.ContainsKey(plugin.Metadata.CommandName))
                {
                    plugins.Add(plugin.Metadata.CommandName, plugin.Value);
                }
                else
                {
                    UIAssistantAPI.NotifyWarnMessage("Warning", string.Format(TextID.PluginCommandDuplication.GetLocalizedText(), plugin.Metadata.Name));
                }
            });

            Localize();
        }

        public Action GenerateAction(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return null;
            }
            var args = command.Split(' ');
            var action = Plugins.Where(plugin => plugin.Metadata.CommandName == args[0]).FirstOrDefault()?.Value.GenerateAction(args);
            return () =>
            {
                InitializeBeforePluginCalled();
                action?.Invoke();
            };
        }

        private void InitializeBeforePluginCalled()
        {
            Resume = null;
            Quit = null;
        }

        public void Execute(string command)
        {
            GenerateAction(command)?.Invoke();
        }

        public bool Exists(string command)
        {
            return GenerateAction(command) != null;
        }

        public event Action Resume;
        public void Undo()
        {
            Resume?.Invoke();
            Resume = null;
        }

        public event Action Quit;
        public void Exit()
        {
            Quit?.Invoke();
            Quit = null;
        }

        public void Localize()
        {
            LocalizablePlugins.ForEach(p => p.Localize());
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposablePlugins.ForEach(plugin => plugin.Dispose());
                    container?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
