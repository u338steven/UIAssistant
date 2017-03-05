using System;
using System.Collections.Generic;
using System.Windows;

using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Infrastructure.Resource
{
    public class ResourceReader<T> : IResourceReader<T> where T : IResourceItem
    {
        public ResourceDictionary Read(T resource)
        {
            return new ResourceDictionary { Source = new Uri(resource.FilePath) };
        }
    }

    public class CacheableResourceReader<T> : IResourceReader<T> where T : IResourceItem
    {
        private IDictionary<string, ResourceDictionary> _cachedDictionaries;

        public CacheableResourceReader()
        {
            _cachedDictionaries = new Dictionary<string, ResourceDictionary>();
        }

        public ResourceDictionary Read(T resource)
        {
            if (!IsCached(resource))
            {
                CreateCache(resource);
            }
            return _cachedDictionaries[resource.Id];
        }

        private bool IsCached(T resource)
        {
            return _cachedDictionaries.ContainsKey(resource.Id);
        }

        private void CreateCache(T resource)
        {
            _cachedDictionaries.Add(resource.Id, new ResourceDictionary { Source = new Uri(resource.FilePath) });
        }
    }
}
