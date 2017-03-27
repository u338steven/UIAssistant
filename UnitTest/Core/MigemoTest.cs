using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UIAssistant.Utility;

namespace UnitTest.Core
{
    [TestClass]
    public class MigemoTest
    {
        [TestMethod]
        public void MigemoAllCharTest()
        {
            if (!Migemo.IsEnable())
            {
                var solutionDir = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName).FullName).FullName).FullName;
                var migemoDllPath = Path.Combine(solutionDir, "assemblies", "Migemo");
                var migemoDictionaryPath = Path.Combine(solutionDir, "assemblies", "Migemo", "dict", "migemo-dict");
                //Console.WriteLine(settings.MigemoDllPath);
                //Console.WriteLine(settings.MigemoDictionaryPath);
                Migemo.Initialize(migemoDllPath, migemoDictionaryPath);
            }
            if (!Migemo.IsEnable())
            {
                // Fail: Migemo initialize
                Assert.Fail();
            }
            for (var test = Convert.ToChar(0); test <= 255; test++)
            {
                try
                {
                    var ret = Migemo.GetRegex(test.ToString());
                    //Console.WriteLine("{0}, {1}", test, ret.ToString());
                }
                catch
                {
                    Assert.Fail();
                }
            }
        }
    }
}
