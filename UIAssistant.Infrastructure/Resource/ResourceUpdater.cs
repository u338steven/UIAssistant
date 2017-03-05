using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;

using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Infrastructure.Resource
{
    public class ResourceUpdater<T> : IResourceUpdater<T> where T : IResourceItem
    {
        private ICollection<ResourceDictionary> _mergedDictionaries;
        private IResourceReader<T> _reader;

        public ResourceUpdater(IResourceReader<T> reader, ICollection<ResourceDictionary> target)
        {
            Contract.Requires(reader != null);
            Contract.Requires(target != null);

            _reader = reader;
            _mergedDictionaries = target;
        }

        public void Update(T oldResource, T newResource)
        {
            RemoveRedundancy(oldResource);
            _mergedDictionaries.Add(_reader.Read(newResource));
        }

        private void RemoveRedundancy(T oldResource)
        {
            _mergedDictionaries
                .Where(d => d.Source != null && d.Source.IsAbsoluteUri && oldResource?.FilePath == d.Source.OriginalString)
                .ToList()
                .ForEach(d => _mergedDictionaries.Remove(d));
        }
    }
}
