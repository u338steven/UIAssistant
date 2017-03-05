using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows;

namespace UIAssistant.Interfaces.Resource
{
    public interface IResourceItem
    {
        string Id { get; }
        string FilePath { get; }
    }

    public interface IResourceKeyValueGenerator<T> where T : IResourceItem
    {
        string GenerateKey(string filePath);
        T GenerateValue(string filePath);
    }

    public interface IResourceDirectory<T>
    {
        IDictionary<string, T> AvailableResources { get; }
    }

    public interface IResourceState<T> where T : IResourceItem
    {
        T Current { get; }
    }

    public interface IResourceFinder<T> where T : IResourceItem
    {
        IDictionary<string, T> Availables { get; }
        bool Exists(string resourceName);
        T Find(string resourceName);
        T FindNext(T current);
    }

    public interface IResourceReader<T>
    {
        ResourceDictionary Read(T resource);
    }

    [ContractClass(typeof(IResourceUpdaterContract<>))]
    public interface IResourceUpdater<T> where T : IResourceItem
    {
        void Update(T oldResource, T newResource);
    }

    [ContractClassFor(typeof(IResourceUpdater<>))]
    internal abstract class IResourceUpdaterContract<T> : IResourceUpdater<T> where T : IResourceItem
    {
        public void Update(T oldResource, T newResource)
        {
            Contract.Requires(newResource != null);
        }
    }

    public interface ILocalizer
    {
        string GetLocalizedText(string key);
        void SwitchLanguage(IResourceItem language);
    }

    public interface ISwitcher
    {
        IResourceItem CurrentTheme { get; }
        void Next();
        void Switch(string id);
    }
}
