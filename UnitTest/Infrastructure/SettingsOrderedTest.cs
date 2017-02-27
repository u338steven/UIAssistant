using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

namespace SettingsTest
{
    [TestClass]
    public class SettingsOrderedTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            if (File.Exists(TestPath.FilePath))
            {
                File.Delete(TestPath.FilePath);
            }
        }

        [TestMethod]
        public void CreateNew()
        {
            Assert.AreEqual(false, File.Exists(TestPath.FilePath));

            TestStorage.Instance.Load();

            Assert.AreEqual(true, File.Exists(TestPath.FilePath));

            Assert.AreEqual(15, TestStorage.Instance.ItemsCountPerPage);
            Assert.AreEqual("General", TestStorage.Instance.Theme);
            Assert.AreEqual(true, TestStorage.Instance.UseMigemo);
        }

        [TestMethod]
        public void ElementRemoved()
        {
            ThemeRemoved.Instance.Load();
            ThemeRemoved.Instance.ItemsCountPerPage = 88;
            ThemeRemoved.Instance.Save();

            TestStorage.Instance.Load();
            Assert.AreEqual(true, TestStorage.Instance.UseMigemo);
            Assert.AreEqual(88, TestStorage.Instance.ItemsCountPerPage);
        }

        [TestMethod]
        public void ElementAdded()
        {
            TestStorage.Instance.Load();
            TestStorage.Instance.Theme = "Added";
            TestStorage.Instance.Save();

            TestStorage.Instance.Load();
            Assert.AreEqual("Added", TestStorage.Instance.Theme);
            Assert.AreEqual(88, TestStorage.Instance.ItemsCountPerPage);
            Assert.AreEqual(true, TestStorage.Instance.UseMigemo);
        }

        [TestMethod]
        public void RaiseException()
        {
            try
            {
                ExceptionRaised.Instance.Load();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"{ex.Message}\n{ex.StackTrace}");
                return;
            }
            Assert.Fail();
        }
    }
}
