using System.Windows;

namespace UIAssistant.Interfaces.Plugin
{
    public interface IPlugin
    {
        void Initialize();
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
}
