using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIAssistant.Core.HitaHint;

namespace UnitTest
{
    [TestClass]
    public class HintGeneratorTest
    {
        delegate IEnumerable<string> GenerateDelegate(string hintKeys, int quantity);

        [TestMethod]
        public void HintGeneratorTest1()
        {
            Enumerable.Range(1, 200).ToList().ForEach(amount =>
            {
                var result = HintGenerator.Generate("asdfhjkl", amount);
                Assert.AreEqual(amount, result.Count());
                Assert.IsFalse(result.Any(x => result.Any(y => y.StartsWith(x) && y != x)));
            });
        }

        [TestMethod]
        public void AlternateHintGeneratorTest1()
        {
            Enumerable.Range(1, 200).ToList().ForEach(amount =>
            {
                var hintKeys = "asdfg|hjkl";
                var result = AlternateHintGenerator.Generate(hintKeys, amount);
                Assert.AreEqual(amount, result.Count());
                Assert.IsFalse(result.Any(x => result.Any(y => y.StartsWith(x) && y != x)));
                Assert.IsTrue(IsValidAlterneation(hintKeys, result));
            });
        }

        enum HintContain
        {
            Null,
            Left,
            Right,
        }

        private bool IsValidAlterneation(string hintKeys, IEnumerable<string> result)
        {
            var a = hintKeys.Split('|');
            var left = a[0];
            var right = a[1];
            var next = HintContain.Null;
            foreach (var item in result)
            {
                next = HintContain.Null;
                foreach (var c in item)
                {
                    if (left.Contains(c) && (next == HintContain.Left || next == HintContain.Null))
                    {
                        next = HintContain.Right;
                        continue;
                    }
                    if (right.Contains(c) && (next == HintContain.Right || next == HintContain.Null))
                    {
                        next = HintContain.Left;
                        continue;
                    }
                    Console.WriteLine($"Check NG!!!!!!!!!!!: {item}");
                    return false;
                }
            }
            return true;
        }
    }
}
