using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace UIAssistant.Infrastructure.Resource
{
    public class ResourceFinder<T> : IResourceFinder<T> where T : IResourceItem
    {
        public IDictionary<string, T> Availables { get; }

        public ResourceFinder(IResourceDirectory<T> dir)
        {
            Contract.Requires(dir != null);

            Availables = dir.AvailableResources;
        }

        public bool Exists(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            return Availables.ContainsKey(fileName);
        }

        public T Find(string fileName)
        {
            Contract.Ensures(Contract.Result<T>() != null);

            if (!Exists(fileName))
            {
                return Availables.ElementAt(0).Value;
            }
            return Availables[fileName];
        }

        public T FindNext(T current)
        {
            Contract.Ensures(Contract.Result<T>() != null);

            var candidate = Availables.SkipWhile(x => !x.Value.Equals(current))?.Skip(1);
            if (candidate.Count() == 0)
            {
                return Availables.ElementAt(0).Value;
            }
            return candidate.ElementAt(0).Value;
        }
    }
}
