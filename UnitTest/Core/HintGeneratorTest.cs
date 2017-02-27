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
            Enumerable.Range(1, 200).ToList().ForEach(x =>
            {
                GeneratorTest(HintGenerator.Generate, x, "asdfhjkl");
            });
        }

        [TestMethod]
        public void AlternateHintGeneratorTest1()
        {
            Enumerable.Range(1, 200).ToList().ForEach(x =>
            {
                GeneratorTest(AlternateHintGenerator.Generate, x, "asdf|hjkl");
            });
        }

        static void GeneratorTest(GenerateDelegate func, int amount, string hintKeys)
        {
            var result = HintGenerator.Generate("asdfghjkl", amount);
            Assert.AreEqual(amount, result.Count());
            Assert.IsFalse(result.Any(x => result.Any(y => y.StartsWith(x) && y != x)));
        }
    }
}
