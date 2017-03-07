using System;
using System.Collections.Generic;

namespace System
{
    public static partial class StringExtensions
    {
        public static bool EqualsWithCaseIgnored(this string strA, string strB)
        {
            return string.Compare(strA, strB, true) == 0;
        }

        public static bool StartsWithCaseIgnored(this string strA, string strB)
        {
            return strA.StartsWith(strB, StringComparison.CurrentCultureIgnoreCase);
        }

        public static IEnumerable<string> Tokenize(this string str, char delimiter = ' ')
        {
            return str.Split(delimiter);
        }
    }
}
