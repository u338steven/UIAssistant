using System;
using System.Collections.Generic;
using System.Linq;

namespace UIAssistant.Infrastructure.Commands
{
    public static partial class StringExtensions
    {
        const char Delimiter = ':';

        public static bool IsOption(this string str)
        {
            return str.StartsWithCaseIgnored("-") || str.StartsWithCaseIgnored("/");
        }

        public static bool HasOptionValue(this string str)
        {
            return str.Contains(Delimiter);
        }

        public static KeyValuePair<string, string> SplitIntoKeyValue(this string word)
        {
            return word.Split(new[] { Delimiter }, 2).Pipe(x => new KeyValuePair<string, string>(x[0], x.ElementAtOrDefault(1)));
        }
    }
}
