using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIAssistant.Utility.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsWithCaseIgnored(this string strA, string strB)
        {
            return string.Compare(strA, strB, true) == 0;
        }

        public static bool StartsWithCaseIgnored(this string strA, string strB)
        {
            return strA.StartsWith(strB, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
