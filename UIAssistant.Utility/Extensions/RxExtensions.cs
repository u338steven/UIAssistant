using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reactive.Linq;

namespace UIAssistant.Utility.Extensions
{
    public static class RxExtensions
    {
        public static IObservable<T> WhereAsync<T>(
            this IObservable<T> source, Func<T, Task<bool>> predicate)
        {
            return source.SelectMany(async item => new
            {
                ShouldInclude = await predicate(item),
                Item = item
            })
                .Where(x => x.ShouldInclude)
                .Select(x => x.Item);
        }

        public static void AddTo(this IDisposable disposable, ICollection<IDisposable> list)
        {
            list.Add(disposable);
        }
    }
}
