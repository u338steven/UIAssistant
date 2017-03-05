using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

using Moq;
using UIAssistant.Infrastructure.Resource;
using UIAssistant.Infrastructure.Resource.Language;
using UIAssistant.Infrastructure.Resource.Theme;
using UIAssistant.Interfaces.Resource;

namespace ResourceTest
{
    [TestClass]
    [DeploymentItem(@"TestData\Themes", @"Themes")]
    [DeploymentItem(@"TestData\Languages", @"Languages")]
    public class ResourceTest
    {
        private ThemeKeyValueGenerator _themeKeyValueGenerator;
        private LanguageKeyValueGenerator _languageKeyValueGenerator;
        private string _themeDir;
        private string _languageDir;
        private Theme _generalTheme;
        private Theme _general2Theme;
        private Theme _solarizedTheme;
        private Dictionary<string, Theme> _themeDic;

        [TestInitialize]
        public void Setup()
        {
            _themeKeyValueGenerator = new ThemeKeyValueGenerator();
            _languageKeyValueGenerator = new LanguageKeyValueGenerator();

            _themeDir = Path.Combine(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString(), @"Themes");
            _languageDir = Path.Combine(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString(), @"Languages");

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            // initialize merged dictionaries
            var d = new DictionaryData();

            var path = _themeDir;
            _generalTheme = new Theme("General", Path.Combine(path, "General.xaml"));
            _general2Theme = new Theme("General2", Path.Combine(path, "General2.xaml"));
            _solarizedTheme = new Theme("Solarized", Path.Combine(path, "Solarized.xaml"));
            _themeDic = new Dictionary<string, Theme>();
            _themeDic.Add("General", _generalTheme);
            _themeDic.Add("General2", _general2Theme);
            _themeDic.Add("Solarized", _solarizedTheme);
        }

        [TestMethod]
        public void ThemeKeyValueGeneratorTest()
        {
            var path = @"C:\foo\bar\foobar.xaml";
            var key = _themeKeyValueGenerator.GenerateKey(path);
            var value = _themeKeyValueGenerator.GenerateValue(path);

            Assert.AreEqual("foobar", key);
            Assert.AreEqual(key, value.Id);
            Assert.AreEqual(path, value.FilePath);
        }

        [TestMethod]
        public void LanguageKeyValueGeneratorTest()
        {
            var path = @"C:\foo\bar\en-US.xaml";
            var key = _languageKeyValueGenerator.GenerateKey(path);
            var value = _languageKeyValueGenerator.GenerateValue(path);

            Assert.AreEqual("en-US", key);
            Assert.AreEqual(key, value.Id);
            Assert.AreEqual(path, value.FilePath);
            Assert.AreEqual("English (United States)", value.DisplayName);
            Assert.AreEqual("English (United States)", value.ToString());
        }

        [TestMethod]
        public void LanguageKeyValueGeneratorTestWhenCannotGetCultureInfo()
        {
            var path = @"C:\foo\bar\foobar.xaml";
            var key = _languageKeyValueGenerator.GenerateKey(path);
            var value = _languageKeyValueGenerator.GenerateValue(path);

            Assert.AreEqual("foobar", key);
            Assert.AreEqual(key, value.Id);
            Assert.AreEqual(path, value.FilePath);
            Assert.AreEqual(key, value.DisplayName);
        }

        [TestMethod]
        public void ResourceDirectoryTest()
        {
            ResourceDirectory<Theme> rd = new ResourceDirectory<Theme>(_themeKeyValueGenerator, _themeDir, "*.xaml");

            Assert.AreEqual(3, rd.AvailableResources.Count);

            AssertResourceDirectoryTest("General", _themeDir, rd.AvailableResources["General"]);
            AssertResourceDirectoryTest("General2", _themeDir, rd.AvailableResources["General2"]);
            AssertResourceDirectoryTest("Solarized", _themeDir, rd.AvailableResources["Solarized"]);
        }

        private void AssertResourceDirectoryTest(string key, string path, Theme actual)
        {
            Assert.AreEqual(key, actual.Id);
            Assert.AreEqual(Path.Combine(path, key + ".xaml"), actual.FilePath);
        }

        [TestMethod]
        public void ResourceFinderTest()
        {
            var dir = new ResourceDirectory<Language>(new LanguageKeyValueGenerator(), _languageDir, "*.xaml");
            var finder = new ResourceFinder<Language>(dir);

            Assert.IsTrue(finder.Exists("ja-JP"));
            Assert.IsFalse(finder.Exists("foo"));

            AssertFinderTest("ja-JP", _languageDir, "Japanese (Japan)", finder);
            AssertFinderTest("en-US", _languageDir, "English (United States)", finder);
            
            Assert.AreEqual("en-US", finder.Availables["en-US"].Id);
            Assert.AreEqual(Path.Combine(_languageDir, "en-US.xaml"), finder.Availables["en-US"].FilePath);
            Assert.AreEqual("en-US2", finder.Availables["en-US2"].Id);
            Assert.AreEqual(Path.Combine(_languageDir, "en-US2.xaml"), finder.Availables["en-US2"].FilePath);
            Assert.AreEqual("ja-JP", finder.Availables["ja-JP"].Id);
            Assert.AreEqual(Path.Combine(_languageDir, "ja-JP.xaml"), finder.Availables["ja-JP"].FilePath);
        }

        private void AssertFinderTest(string id, string dir, string displayName, IResourceFinder<Language> finder)
        {
            var actual = finder.Find(id);
            var path = Path.Combine(dir, id + ".xaml");
            Assert.AreEqual(id, actual.Id);
            Assert.AreEqual(path, actual.FilePath);
            Assert.AreEqual(displayName, actual.DisplayName);
        }

        [TestMethod]
        public void ResourceFinderNullTest()
        {
            var dir = new ResourceDirectory<Language>(new LanguageKeyValueGenerator(), _languageDir, "*.xaml");
            var finder = new ResourceFinder<Language>(dir);

            var result = finder.Find(null);

            Assert.AreEqual("en-US", result.Id);
            Assert.AreEqual("English (United States)", result.DisplayName);
        }

        [TestMethod]
        public void ResourceReaderTest()
        {
            var reader = new ResourceReader<Theme>();

            var result = reader.Read(_themeDic["General"]);
            Assert.AreEqual("General theme", result["ThemeName"]);

            result = reader.Read(_themeDic["Solarized"]);
            Assert.AreEqual("Solarized theme", result["ThemeName"]);

            result = reader.Read(_themeDic["General"]);
            Assert.AreEqual("General theme", result["ThemeName"]);
        }

        [TestMethod]
        public void CacheableResourceReaderTest()
        {
            var reader = new CacheableResourceReader<Theme>();

            PrivateObject po = new PrivateObject(reader);
            var cachedDic = (po.GetField("_cachedDictionaries") as IDictionary<string, ResourceDictionary>);

            var result = reader.Read(_themeDic["General"]);
            Assert.AreEqual("General theme", result["ThemeName"]);
            Assert.AreEqual(1, cachedDic.Count);

            result = reader.Read(_themeDic["Solarized"]);
            Assert.AreEqual("Solarized theme", result["ThemeName"]);
            Assert.AreEqual(2, cachedDic.Count);

            result = reader.Read(_themeDic["General"]);
            Assert.AreEqual("General theme", result["ThemeName"]);
            Assert.AreEqual(2, cachedDic.Count);

            result = reader.Read(_themeDic["Solarized"]);
            Assert.AreEqual("Solarized theme", result["ThemeName"]);
            Assert.AreEqual(2, cachedDic.Count);
        }

        [TestMethod]
        public void ResourceUpdaterTest()
        {
            var r = new ResourceDictionary();
            var mergedDictionaries = r.MergedDictionaries;

            var reader = new CacheableResourceReader<Theme>();
            var updater = new ResourceUpdater<Theme>(reader, mergedDictionaries);

            Assert.AreEqual(0, mergedDictionaries.Count);

            // General to General (update, merge)
            updater.Update(_generalTheme, _generalTheme);
            Assert.AreEqual(1, mergedDictionaries.Count);

            // General to Solarized (update, merge, remove)
            updater.Update(_generalTheme, _solarizedTheme);
            Assert.AreEqual(1, mergedDictionaries.Count);
            Assert.AreEqual("Solarized theme", mergedDictionaries[0]["ThemeName"] as string);

            // Solarized to General2 (update, merge, remove)
            updater.Update(_solarizedTheme, _general2Theme);
            Assert.AreEqual(1, mergedDictionaries.Count);
            Assert.AreEqual("General2 theme", mergedDictionaries[0]["ThemeName"] as string);
        }

        [TestMethod]
        public void ResourceSwitcherTest1()
        {
            int updateCount = 0;
            var updater = new Mock<IResourceUpdater<Theme>>();
            updater.Setup(x => x.Update(It.IsAny<Theme>(), It.IsAny<Theme>()))
                .Callback((Theme old, Theme x) => ++updateCount);

            var dir = new Mock<IResourceDirectory<Theme>>();
            dir.Setup(x => x.AvailableResources).Returns(_themeDic);
            var finder = new ResourceFinder<Theme>(dir.Object);

            var switcher = new ResourceState<Theme>(updater.Object);
            switcher.Switch(finder, "General");
            Assert.AreEqual("General", switcher.Current.Id);
            Assert.AreEqual(1, updateCount);

            switcher.Switch(finder, "Solarized");
            Assert.AreEqual("Solarized", switcher.Current.Id);
            Assert.AreEqual(2, updateCount);

            switcher.Switch(finder, "General");
            Assert.AreEqual("General", switcher.Current.Id);
            Assert.AreEqual(3, updateCount);

            switcher.Switch(finder, "Solarized");
            Assert.AreEqual("Solarized", switcher.Current.Id);
            Assert.AreEqual(4, updateCount);

            switcher.Switch(finder, "Solarized");
            Assert.AreEqual("Solarized", switcher.Current.Id);
            Assert.AreEqual(5, updateCount);

            switcher.Switch(finder, "General");
            Assert.AreEqual("General", switcher.Current.Id);
            Assert.AreEqual(6, updateCount);
        }

        [TestMethod]
        public void LanguageTest()
        {
            var dir = new ResourceDirectory<Language>(new LanguageKeyValueGenerator(), _languageDir, "*.xaml");
            var finder = new ResourceFinder<Language>(dir);

            var reader = new CacheableResourceReader<Language>();
            var r = new ResourceDictionary();
            var mergedDictionaries = r.MergedDictionaries;
            var switcher = new ResourceState<Language>(new ResourceUpdater<Language>(reader, mergedDictionaries));

            switcher.Switch(finder, "en-US");
            Assert.AreEqual("en-US", switcher.Current.Id);

            switcher.Switch(finder, "ja-JP");
            Assert.AreEqual("ja-JP", switcher.Current.Id);

            switcher.Switch(finder, null);
            Assert.AreEqual("en-US", switcher.Current.Id);
        }

        [TestMethod]
        public void ResourceCompositeTest()
        {
            var dir = new ResourceDirectory<Theme>(new ThemeKeyValueGenerator(), _themeDir, "*.xaml");
            var finder = new ResourceFinder<Theme>(dir);

            var reader = new CacheableResourceReader<Theme>();
            var r = new ResourceDictionary();
            var mergedDictionaries = r.MergedDictionaries;
            var switcher = new ResourceState<Theme>(new ResourceUpdater<Theme>(reader, mergedDictionaries));

            PrivateObject po = new PrivateObject(reader);
            var cachedDic = (po.GetField("_cachedDictionaries") as IDictionary<string, ResourceDictionary>);

            Assert.AreEqual(0, cachedDic.Count);

            switcher.Switch(finder, "General");
            Assert.AreEqual("General", switcher.Current.Id);
            Assert.AreEqual(1, cachedDic.Count);

            switcher.Switch(finder, "Solarized");
            Assert.AreEqual("Solarized", switcher.Current.Id);
            Assert.AreEqual(2, cachedDic.Count);

            switcher.SwitchNext(finder);
            Assert.AreEqual("General", switcher.Current.Id);
            Assert.AreEqual(2, cachedDic.Count);

            switcher.Switch(finder, "Solarized");
            Assert.AreEqual("Solarized", switcher.Current.Id);
            Assert.AreEqual(2, cachedDic.Count);

            switcher.SwitchNext(finder);
            Assert.AreEqual("General", switcher.Current.Id);
            Assert.AreEqual(2, cachedDic.Count);

            switcher.SwitchNext(finder);
            Assert.AreEqual("General2", switcher.Current.Id);
            Assert.AreEqual(3, cachedDic.Count);

            switcher.SwitchNext(finder);
            Assert.AreEqual("Solarized", switcher.Current.Id);
            Assert.AreEqual(3, cachedDic.Count);

            switcher.SwitchNext(finder);
            Assert.AreEqual("General", switcher.Current.Id);
            Assert.AreEqual(3, cachedDic.Count);
        }
    }
}
