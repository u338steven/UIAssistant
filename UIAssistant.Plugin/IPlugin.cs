using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using UIAssistant.Core.Enumerators;

namespace UIAssistant.Plugin
{
    public interface IPlugin
    {
        void Initialize();
        Action GenerateAction(IList<string> args);
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
