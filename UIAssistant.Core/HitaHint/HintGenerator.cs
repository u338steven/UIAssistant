using System;
using System.Collections.Generic;
using System.Linq;

namespace UIAssistant.Core.HitaHint
{
    public static class HintGenerator
    {
        static string _defaultKeys = "asdfghjkl";

        public static IEnumerable<string> Generate(string hintKeys, int quantity)
        {
            if (quantity <= 0)
            {
                return null;
            }

            if (hintKeys.Length < 2)
            {
                hintKeys = _defaultKeys;
            }

            var len = hintKeys.Length;
            int maxLength = 0;
            while (quantity > Math.Pow(len, maxLength)){
                ++maxLength;
            }
            if (maxLength == 1)
            {
                return hintKeys.Substring(0, quantity).Select(x => x.ToString()).ToList();
            }
            var hintSources = GenerateHintSources(hintKeys, maxLength - 1);

            var unit = (int)Math.Pow(len, maxLength - 1);
            var maxLengthQuantity = quantity / unit;
            if (quantity % unit > len - maxLengthQuantity)
            {
                maxLengthQuantity++;
            }
            var remainder = quantity - hintSources.Count() + maxLengthQuantity;

            var maxLengthHints = GenerateMaxLengthHints(hintKeys.Substring(0, maxLengthQuantity), hintSources, remainder);
            var results = hintSources.Skip(maxLengthQuantity).ToList();
            results.AddRange(maxLengthHints);
            return results;
        }

        private static IEnumerable<string> GenerateMaxLengthHints(string partialHintSource, IEnumerable<string> hintSource, int quantity)
        {
            var result = new List<string>();
            var len = partialHintSource.Length;//6
            var len2 = hintSource.Count();

            for (var i = 0; i < len; ++i)
            {
                for (var j = 0; j < len2; ++j)
                {
                    result.Add($"{partialHintSource[i]}{hintSource.ElementAt(j)}");
                    --quantity;
                    if (quantity <= 0)
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        private static IEnumerable<string> GenerateHintSources(string hintKeys, int length)
        {
            if (length <= 1)
            {
                return hintKeys.Select(x => x.ToString()).ToList();
            }

            var result = new List<string>();
            var results = GenerateHintSources(hintKeys, length - 1).ToList();
            var len = hintKeys.Length;
            results.ForEach(x =>
            {
                for (var i = 0; i < len; ++i)
                {
                    result.Add($"{x}{hintKeys[i]}");
                }
            });
            return result;
        }
    }
}
