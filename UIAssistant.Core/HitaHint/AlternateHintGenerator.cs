using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIAssistant.Core.HitaHint
{
    public static class AlternateHintGenerator
    {
        static string _defaultKeys = "asdfg|hjkl";

        public static IEnumerable<string> Generate(string hintKeys, int quantity)
        {
            if (quantity <= 0)
            {
                return null;
            }

            if (!hintKeys.Contains('|'))
            {
                return HintGenerator.Generate(hintKeys, quantity);
            }

            if (hintKeys.Length < 3)
            {
                hintKeys = _defaultKeys;
            }

            var hintSources = hintKeys.Split('|');
            var leftHintSource = hintSources[0];
            var rightHintSource = hintSources[1];

            return GenerateAlternateHints(leftHintSource, rightHintSource, quantity);
        }

        private static IEnumerable<string> GenerateAlternateHints(string leftSource, string rightSource, int quantity)
        {
            var maxLength = 1d;
            while (Math.Pow((leftSource.Length * rightSource.Length), Math.Floor(maxLength / 2)) * (maxLength % 2 == 1 ? (leftSource.Length + rightSource.Length) : 2) < quantity)
            {
                ++maxLength;
            }

            if (maxLength <= 1)
            {
                var hintSource = string.Concat(leftSource, rightSource);
                return hintSource.Substring(0, quantity).Select(x => x.ToString()).ToList();
            }

            var partialHints1 = GenerateAlternateHintsInternal(leftSource, rightSource, (int)maxLength - 1);
            var partialHints2 = GenerateAlternateHintsInternal(rightSource, leftSource, (int)maxLength - 1);

            int used;

            var remainder = quantity - partialHints1.Count() - partialHints2.Count();
            var maxLengthResults1 = GenerateAlternateHintsInternal(leftSource, partialHints1, ref remainder, out used);
            var minLengthResults1 = partialHints1.Skip(used);

            var maxLengthResults2 = GenerateAlternateHintsInternal(rightSource, partialHints2, ref remainder, out used);
            var minLengthResults2 = partialHints2.Skip(used);

            return minLengthResults1.Concat(minLengthResults2).Concat(maxLengthResults1).Concat(maxLengthResults2);
        }

        private static IEnumerable<string> GenerateAlternateHintsInternal(string partialHintSource, IEnumerable<string> hintSource, ref int quantity, out int used)
        {
            var result = new List<string>();
            var len = hintSource.Count();
            var len2 = partialHintSource.Length;
            used = 0;

            if (quantity <= 0)
            {
                return result;
            }

            for (var i = 0; i < len; ++i)
            {
                ++used;
                ++quantity;
                for (var j = 0; j < len2; ++j)
                {
                    result.Add($"{hintSource.ElementAt(i)}{partialHintSource[j]}");
                    --quantity;
                    if (quantity <= 0)
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        private static IEnumerable<string> GenerateAlternateHintsInternal(string source1, string source2, int length)
        {
            if (length < 2)
            {
                return (source1).Select(x => x.ToString());
            }
            else if (length == 2)
            {
                return source1.SelectMany(x => source2.Select(y => $"{x}{y}"));
            }

            --length;
            return source1.SelectMany(x => GenerateAlternateHintsInternal(source2, source1, length).Select(y => $"{x}{y}"));
        }
    }
}
