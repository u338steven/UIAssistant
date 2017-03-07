using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static TR Pipe<T, TR>(this T self, Func<T, TR> func)
        {
            return func(self);
        }

        public static T Tap<T>(this T self, Action<T> action)
        {
            action(self);
            return self;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            source.ToList().ForEach(item => action(item));
        }
    }
}
