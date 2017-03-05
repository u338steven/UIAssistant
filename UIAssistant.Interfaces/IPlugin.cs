using System;
using System.Collections.Generic;
using System.Windows;
using UIAssistant.Interfaces.API;

namespace UIAssistant.Interfaces.Plugin
{
    public interface IPlugin
    {
        void Initialize(IUIAssistantAPI api);
        void Setup();
    }

    public interface IConfigurablePlugin
    {
        FrameworkElement GetConfigrationInterface();
        void Save();
    }

    public interface IPluginMetadata
    {
        string Guid { get; }
        string Name { get; }
        string Author { get; }
        string SupportUri { get; }
        string IconUri { get; }
        string Version { get; }
        string CommandName { get; }
    }

    public interface ILocalizablePlugin
    {
        void Localize();
    }

    public interface IPluginManager
    {
        IEnumerable<Lazy<IPlugin, IPluginMetadata>> Plugins { get; set; }

        event Action Quit;
        event Action Resume;

        void Dispose();
        void Execute(string command);
        bool Exists(string command);
        void Exit();
        Action GenerateAction(string command);
        void Localize();
        void ResetAllPlugins();
        void Undo();
    }
}
