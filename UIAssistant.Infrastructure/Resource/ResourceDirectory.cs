using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Infrastructure.Resource
{
    public class ResourceDirectory<T> : IResourceDirectory<T> where T : IResourceItem
    {
        public IDictionary<string, T> AvailableResources { get; }

        public ResourceDirectory(IResourceKeyValueGenerator<T> generator, string directoryPath, string searchPattern = "*.*")
        {
            Contract.Requires(generator != null);
            Contract.Requires(directoryPath != null);
            Contract.Requires(searchPattern != null);

            AvailableResources = Directory.GetFiles(directoryPath, searchPattern)
                .ToDictionary(x => generator.GenerateKey(x), x => generator.GenerateValue(x));
        }
    }
}
