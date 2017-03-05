using System.Diagnostics.Contracts;

using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Infrastructure.Resource
{
    public class ResourceState<T> : IResourceState<T> where T : IResourceItem
    {
        public T Current { get; private set; }
        private IResourceUpdater<T> _updater;

        public ResourceState(IResourceUpdater<T> updater)
        {
            Contract.Requires(updater != null);

            _updater = updater;
        }

        public void Switch(IResourceFinder<T> finder, string id)
        {
            Contract.Requires(finder != null);
            Contract.Ensures(Current != null);

            var newResource = finder.Find(id);
            _updater.Update(Current, newResource);
            Current = newResource;
        }

        public void SwitchNext(IResourceFinder<T> finder)
        {
            Contract.Requires(finder != null);
            Contract.Ensures(Current != null);

            var newResource = finder.FindNext(Current);
            _updater.Update(Current, finder.FindNext(Current));
            Current = newResource;
        }
    }
}
