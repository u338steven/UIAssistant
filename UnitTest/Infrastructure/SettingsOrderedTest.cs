using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using UIAssistant.Infrastructure.Settings;
using UIAssistant.Interfaces.Settings;

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

            if (Directory.Exists(FoobarPath.DirPath))
            {
                Directory.Delete(FoobarPath.DirPath, true);
            }
        }

        private void AssertExceptionFail(string filePath, Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"Path: {filePath}");
            System.Diagnostics.Trace.WriteLine($"Message: {ex.Message}");
            Assert.Fail();
        }

        [TestMethod]
        public void CreateNew()
        {
            Assert.AreEqual(false, File.Exists(TestPath.FilePath));

            YamlFileIO file = new YamlFileIO(AssertExceptionFail);
            var data = file.Read(typeof(TestStorage), TestPath.FilePath) as TestStorage;

            Assert.AreEqual(true, File.Exists(TestPath.FilePath));

            Assert.AreEqual(15, data.ItemsCountPerPage);
            Assert.AreEqual("General", data.Theme);
            Assert.AreEqual(true, data.UseMigemo);
        }

        [TestMethod]
        public void ElementRemoved()
        {
            YamlFileIO file = new YamlFileIO(AssertExceptionFail);
            var data = file.Read(typeof(ThemeRemoved), TestPath.FilePath) as ThemeRemoved;
            data.ItemsCountPerPage = 88;
            file.Write(typeof(ThemeRemoved), data, TestPath.FilePath);

            var newData = file.Read(typeof(ThemeRemoved), TestPath.FilePath) as ThemeRemoved;
            Assert.AreEqual(true, newData.UseMigemo);
            Assert.AreEqual(88, newData.ItemsCountPerPage);
        }

        [TestMethod]
        public void ElementAdded()
        {
            YamlFileIO file = new YamlFileIO(AssertExceptionFail);
            var data = file.Read(typeof(TestStorage), TestPath.FilePath) as TestStorage;

            data.Theme = "Added";
            file.Write(typeof(TestStorage), data, TestPath.FilePath);

            var newData = file.Read(typeof(TestStorage), TestPath.FilePath) as TestStorage;
            Assert.AreEqual("Added", newData.Theme);
            Assert.AreEqual(88, newData.ItemsCountPerPage);
            Assert.AreEqual(true, newData.UseMigemo);
        }

        [TestMethod]
        public void RaiseException()
        {
            try
            {
                YamlFileIO file = new YamlFileIO((filename, ex) =>
                {
                    throw ex;
                });
                var data = file.Read(typeof(ExceptionRaised), TestPath.FilePath) as ExceptionRaised;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"{ex.Message}\n{ex.StackTrace}");
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void WriteYamlFileTest()
        {
            FooSettings foo = new FooSettings() { Foo = 10, Bar = 20 };
            IFileIO writer = new YamlFileIO();
            writer.Write(typeof(FooSettings), foo, FoobarPath.FilePath);

            Assert.AreEqual(true, File.Exists(FoobarPath.FilePath));
        }

        [TestMethod]
        public void ReadYamlFileTest()
        {
            IFileIO reader = new YamlFileIO();
            FooSettings foo = reader.Read(typeof(FooSettings), FoobarPath.FilePath) as FooSettings;
            Assert.AreEqual(10, foo.Foo);
            Assert.AreEqual(20, foo.Bar);
        }

        [TestMethod]
        public void ScenarioTest()
        {
            IFileIO io = new YamlFileIO();
            var loader = new SettingsLoader<FooSettings>(io, FoobarPath.FilePath);
            var foo = loader.Settings;

            Assert.AreEqual(10, foo.Foo);
            Assert.AreEqual(20, foo.Bar);
        }
    }
}
