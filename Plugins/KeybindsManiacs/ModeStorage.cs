using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    class ModeStorage
    {
        public string ModeName { get; set; }
        public bool IsEnabledWindowsKeybinds { get; set; }
        public bool IsPrefix { get; set; }
        public string ProcessName { get; set; }
        public string ClassName { get; set; }
        public ObservableCollection<KeyTranslator> Keybinds { get; private set; } = new ObservableCollection<KeyTranslator>();

        public ModeStorage()
        {

        }

        public ModeStorage(bool isEnabledWindowsKeybinds, bool isPrefix = false)
        {
            IsEnabledWindowsKeybinds = isEnabledWindowsKeybinds;
            IsPrefix = isPrefix;
        }

        public void Add(KeyTranslator keybind)
        {
            Keybinds.Add(keybind);
        }
    }
}
