﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using UIAssistant.Core.API;
using UIAssistant.Core.I18n;
using UIAssistant.Infrastructure.Logger;
using UIAssistant.Interfaces;
using UIAssistant.Interfaces.Plugin;

using ZoneIdentifierManager;

namespace UIAssistant.Core.Plugin
{
    public class PluginManager : IDisposable, IPluginManager
    {
        public static PluginManager Instance { get; } = new PluginManager();
        private CompositionContainer container;
        private Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();

        private string directoryPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        public IEnumerable<Lazy<IPlugin, IPluginMetadata>> Plugins { get; set; }

        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        private IEnumerable<ILocalizablePlugin> LocalizablePlugins { get; set; }

        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        private IEnumerable<IDisposable> DisposablePlugins { get; set; }

        private PluginManager() { }

        public void Initialize()
        {
            var catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            var pluginPaths = Directory.EnumerateFiles(directoryPath, "*.dll", SearchOption.AllDirectories);

            pluginPaths.ForEach(pluginPath =>
            {
                try
                {
                    if (ZoneIdentifier.IsBlocked(pluginPath))
                    {
                        if (UIAssistantAPI.Instance.ViewAPI.GetConfirmation("The plugin is blocked",
                            string.Format(TextID.PluginBlocked.GetLocalizedText(), Path.GetFileName(pluginPath), pluginPath)))
                        {
                            ZoneIdentifier.Unblock(pluginPath);
                        }
                    }
                }
                catch
                {

                }

                try
                {
                    var asmCatalog = new AssemblyCatalog(pluginPath);
                    if (asmCatalog.Parts.ToList().Count > 0)
                    {
                        catalog.Catalogs.Add(asmCatalog);
                    }
                }
                catch (FileLoadException ex)
                {
                    var message = string.Format(TextID.PluginInitializeError.GetLocalizedText(), pluginPath);
                    UIAssistantAPI.Instance.NotificationAPI.NotifyWarnMessage("Warning", message);
                    Log.Error(ex);
                    Log.Warn(message);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var message = string.Format(TextID.PluginInitializeError.GetLocalizedText(), pluginPath);
                    UIAssistantAPI.Instance.NotificationAPI.NotifyWarnMessage("Warning", message);
                    (ex as ReflectionTypeLoadException).LoaderExceptions.ForEach(x =>
                    {
                        Log.Error(x);
                    });
                }
                catch (Exception ex)
                {
                    var message = string.Format(TextID.PluginInitializeError.GetLocalizedText(), pluginPath);
                    UIAssistantAPI.Instance.NotificationAPI.NotifyWarnMessage("Warning", message);
                    Log.Error(ex);
                    Log.Warn(message);
                }
            });

            container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            LoadAllPlugins();

            RemoveRemovedPlugins();
        }

        private void RemoveRemovedPlugins()
        {
            var settings = UIAssistantAPI.Instance.UIAssistantSettings;

            var oldDisabledPlugins = new HashSet<string>(settings.DisabledPlugins);
            oldDisabledPlugins.ForEach(x =>
            {
                if (!Plugins.Any(y => y.Metadata.Guid.EqualsWithCaseIgnored(x)))
                {
                    settings.DisabledPlugins.Remove(x);
                }
            });
        }

        private void LoadAllPlugins()
        {
            var settings = UIAssistantAPI.Instance.UIAssistantSettings;

            Plugins.ForEach(plugin =>
            {
                if (settings.DisabledPlugins.Contains(plugin.Metadata.Guid))
                {
                    return;
                }
                Load(plugin);
            });
        }

        public void ResetAllPlugins()
        {
            DisposablePlugins.ForEach(plugin => plugin.Dispose());
            plugins = new Dictionary<string, IPlugin>();
            LoadAllPlugins();
            Localize();
        }

        public Action GenerateAction(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return null;
            }
            var args = command.Split(' ');
            if (!plugins.ContainsKey(args[0]))
            {
                return null;
            }
            var actions = UIAssistantAPI.Instance.CommandAPI.ParseStatement(command).Reverse();
            return () =>
            {
                try
                {
                    InitializeBeforePluginCalled();
                    plugins[args[0]].Setup();
                    actions.ForEach(x => x.Invoke());
                }
                catch (Exception ex)
                {
                    var pluginName = Plugins.FirstOrDefault(x => x.Value == plugins[args[0]]).Metadata.Name;
                    UIAssistantAPI.Instance.NotificationAPI.NotifyErrorMessage(pluginName, string.Format(TextID.PluginError.GetLocalizedText(), pluginName));
                    Log.Error(ex, pluginName);
                }
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

        private void Load(Lazy<IPlugin, IPluginMetadata> plugin)
        {
            try
            {
                plugin.Value.Initialize(UIAssistantAPI.Instance);
            }
            catch (Exception ex)
            {
                var message = string.Format(TextID.PluginInitializeError.GetLocalizedText(), plugin.Metadata.Name);
                UIAssistantAPI.Instance.NotificationAPI.NotifyWarnMessage("Warning", message);
                Log.Error(ex);
                Log.Warn(message);
            }
            if (!plugins.ContainsKey(plugin.Metadata.CommandName))
            {
                plugins.Add(plugin.Metadata.CommandName, plugin.Value);
            }
            else
            {
                UIAssistantAPI.Instance.NotificationAPI.NotifyWarnMessage("Warning", string.Format(TextID.PluginCommandDuplication.GetLocalizedText(), plugin.Metadata.Name));
            }
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
