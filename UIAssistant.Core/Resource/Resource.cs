using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Windows;
using UIAssistant.Core.Settings;
using UIAssistant.Core.I18n;
using UIAssistant.UI.Controls;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Core.Resource
{
    public interface IResourceItem
    {
        string FileName { get; }
    }

    public abstract class Resource<T> where T : IResourceItem
    {
        public T Current;
        public virtual string Default => "default";
        protected virtual string DirectoryPath => UIAssistantDirectory.Executable;
        protected IList<T> AvailableDictionaries;
        private ResourceDictionary _appliedDict;

        public bool IsAlreadyApplied(T dic)
        {
            return Current.FileName == dic.FileName;
        }

        public void Switch(T dic)
        {
            if (Current != null && IsAlreadyApplied(dic))
            {
                if (_appliedDict != null)
                {
                    Remove(Current.FileName);
                    Application.Current.Resources.MergedDictionaries.Add(_appliedDict);
                }
                return;
            }
            var path = GetDictionaryPath(dic);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var removeTarget = Current;
            Current = dic;
            Application.Current.Resources.MergedDictionaries.Add(GetDictionary());

            if (removeTarget != null && removeTarget.FileName != dic.FileName)
            {
                Remove(removeTarget.FileName);
            }
        }

        public IList<T> GetAvailables()
        {
            if (AvailableDictionaries == null)
            {
                InitAvailableDictionaries();
            }
            return AvailableDictionaries;
        }

        public void InitAvailableDictionaries()
        {
            var files = Directory.GetFiles(DirectoryPath, "*.xaml").Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
            CreateAvailableDictionaries(files);
        }

        protected abstract void CreateAvailableDictionaries(string[] files);

        public T Find(string name)
        {
            return GetAvailables().FirstOrDefault(c => c.FileName == name);
        }

        public void Next()
        {
            var candidate = GetAvailables().SkipWhile(x => x.FileName != Current?.FileName)?.Skip(1);
            if (candidate == null || candidate.Count() == 0)
            {
                Switch(GetAvailables()[0]);
                return;
            }
            Switch(candidate.ElementAt(0));
        }

        public void Remove(string fileName)
        {
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var fullPath = $"{DirectoryPath}{Path.DirectorySeparatorChar}{fileName}.xaml";
            dictionaries.Where(d => fullPath == d.Source.LocalPath).ForEach(d => dictionaries.Remove(d));
        }

        public ResourceDictionary GetDictionary()
        {
            string path = GetDictionaryPath(Current);
            if (string.IsNullOrEmpty(path))
            {
                Current = Find(Default);
                path = GetDictionaryPath(Current);
                if (string.IsNullOrEmpty(path))
                {
                    return new ResourceDictionary();
                }
            }
            _appliedDict = new ResourceDictionary();
            try
            {
                _appliedDict.Source = new Uri(path, UriKind.Absolute);
            }
            catch
            {
                Notification.NotifyMessage("Load Theme Error", string.Format(TextID.LoadThemeError.GetLocalizedText(), Current.FileName), NotificationIcon.Error);
            }
            return _appliedDict;
        }

        private string GetDictionaryPath(T item)
        {
            string path = Path.Combine(DirectoryPath, item?.FileName + ".xaml");
            if (File.Exists(path))
            {
                return path;
            }

            return string.Empty;
        }
    }
}
