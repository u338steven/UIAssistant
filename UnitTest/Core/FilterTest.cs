using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UIAssistant.Core.API;
using UIAssistant.Core.Enumerators;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Utility;

namespace UnitTest.Core
{
    [TestClass]
    public class FilterTest
    {
        private static IFilter _filter;
        private static List<IHUDItem> _collection = new List<IHUDItem>();

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Application apl = new Application();
            Application.Current.Resources = new ResourceDictionary();
            for (int i = 0; i < 9; i++)
            {
                _collection.Add(new TestItem() { InternalText = $"internal{i}", DisplayText = $"display{i} 画面" });
            }
            _collection.Add(new TestItem() { DisplayText = $"здравствуйте 1234567890" });
            _filter = new DefaultFilter();
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            Migemo.Dispose();
        }

        [TestMethod]
        public void EmptyStringTest()
        {
            var results = _filter.Filter(_collection, "");
            Assert.AreEqual(10, results.Count());
        }

        [TestMethod]
        public void NotFoundTest()
        {
            var results = _filter.Filter(_collection, "int");
            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void FoundTest()
        {
            var results = _filter.Filter(_collection, "dis");
            Assert.AreEqual(9, results.Count());
        }

        [TestMethod]
        public void InputsTest()
        {
            var results = _filter.Filter(_collection, "dis 1");
            Assert.AreEqual(1, results.Count());

            results = _filter.Filter(_collection, "pl 2");
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void MigemoTest()
        {
            if (!Migemo.IsEnable())
            {
                var solutionDir = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName).FullName).FullName).FullName;
                var settings = UIAssistantAPI.Instance.UIAssistantSettings;
                settings.UseMigemo = true;
                settings.MigemoDllPath = Path.Combine(solutionDir, "assemblies", "Migemo");
                settings.MigemoDictionaryPath = Path.Combine(solutionDir, "assemblies", "Migemo", "dict", "migemo-dict");
            }
            var migemoFilter = new DefaultFilter();
            if (!Migemo.IsEnable())
            {
                // Fail: Migemo initialize
                Assert.Fail();
            }
            var results = migemoFilter.Filter(_collection, "gamen");
            Assert.AreEqual(9, results.Count());

            results = migemoFilter.Filter(_collection, "здравствуйте");
            Assert.AreEqual(1, results.Count());
        }

        class TestItem : IHUDItem
        {
            public string InternalText { get; set; }
            public string DisplayText { get; set; }
            public Point Location { get; set; }
            public Rect Bounds { get; set; }
            public ImageSource Image { get; }
            public int ColoredStart { get; set; }
            public int ColoredLength { get; set; }

            public void Execute()
            {
            }
        }
    }
}
