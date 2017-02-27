using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Windows.Input;
using UIAssistant.Core.I18n;
using UIAssistant.Core.Settings;
using UIAssistant.Infrastructure.Settings;
using UIAssistant.UI.Controls;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    class KeybindsManiacsSettings : Settings<KeybindsManiacsSettings>
    {
        private const string FileName = "KeybindsManiacs.yml";
        private IFileIO<KeybindsManiacsSettings> _fileIO = new YamlFile<KeybindsManiacsSettings>(UIAssistantDirectory.Configurations, FileName);
        protected override IFileIO<KeybindsManiacsSettings> FileIO { get { return _fileIO; } }

        public ObservableDictionary<string, ModeStorage> KeybindsInMode { get; private set; } = new ObservableDictionary<string, ModeStorage>();
        public string Mode { get; set; } = Consts.DefaultMode;
        public bool RunAtStartup { get; set; } = false;

        public KeybindsManiacsSettings()
        {
            OnError = ex => Notification.NotifyMessage("Load Settings Error", string.Format(TextID.SettingsLoadError.GetLocalizedText(), FileName), NotificationIcon.Warning);
        }

        protected override void LoadDefault()
        {
            var normalMode = new ModeStorage();
            normalMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.OemOpenBrackets }, CommandType.RunEmbeddedCommand, "Cancel"));

            normalMode.Add(new KeyTranslator(new[] { Key.J }, new[] { Key.Down }));
            normalMode.Add(new KeyTranslator(new[] { Key.K }, new[] { Key.Up }));
            normalMode.Add(new KeyTranslator(new[] { Key.H }, new[] { Key.Left }));
            normalMode.Add(new KeyTranslator(new[] { Key.L }, new[] { Key.Right }));

            normalMode.Add(new KeyTranslator(new[] { Key.W }, new[] { Key.RightCtrl, Key.Right }));
            normalMode.Add(new KeyTranslator(new[] { Key.B }, new[] { Key.RightCtrl, Key.Left }));

            normalMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.F }, new[] { Key.PageDown }));
            normalMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.B }, new[] { Key.PageUp }));

            normalMode.Add(new KeyTranslator(new[] { Key.LeftShift, Key.D4 }, new[] { Key.End }));
            normalMode.Add(new KeyTranslator(new[] { Key.D0 }, new[] { Key.Home }));

            normalMode.Add(new KeyTranslator(new[] { Key.LeftShift, Key.X }, new[] { Key.Back }));
            normalMode.Add(new KeyTranslator(new[] { Key.X }, new[] { Key.Delete }));

            normalMode.Add(new KeyTranslator(new[] { Key.U }, new[] { Key.RightCtrl, Key.Z }));
            normalMode.Add(new KeyTranslator(new[] { Key.Y }, CommandType.RunEmbeddedCommand, "VimLike y"));
            normalMode.Add(new KeyTranslator(new[] { Key.P }, new[] { Key.RightCtrl, Key.V }));

            normalMode.Add(new KeyTranslator(new[] { Key.F }, CommandType.RunEmbeddedCommand, "VimLike f"));
            normalMode.Add(new KeyTranslator(new[] { Key.T }, CommandType.RunEmbeddedCommand, "VimLike t"));
            normalMode.Add(new KeyTranslator(new[] { Key.F, Key.LeftShift }, CommandType.RunEmbeddedCommand, "VimLike F"));
            normalMode.Add(new KeyTranslator(new[] { Key.T, Key.LeftShift }, CommandType.RunEmbeddedCommand, "VimLike T"));
            if (UIAssistantAPI.UIAssistantSettings.Culture == "ja-JP")
            {
                normalMode.Add(new KeyTranslator(new[] { Key.OemPlus }, CommandType.RunEmbeddedCommand, "VimLike ;"));
            }
            else
            {
                normalMode.Add(new KeyTranslator(new[] { Key.OemSemicolon }, CommandType.RunEmbeddedCommand, "VimLike ;"));
            }
            normalMode.Add(new KeyTranslator(new[] { Key.OemComma }, CommandType.RunEmbeddedCommand, "VimLike ,"));

            normalMode.Add(new KeyTranslator(new[] { Key.I }, CommandType.RunEmbeddedCommand, "ChangeMode Insert"));
            normalMode.Add(new KeyTranslator(new[] { Key.V }, CommandType.RunEmbeddedCommand, "ChangeMode Visual"));

            normalMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.H }, CommandType.RunUIAssistantCommand, "sn Left"));
            normalMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.J }, CommandType.RunUIAssistantCommand, "sn Down"));
            normalMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.K }, CommandType.RunUIAssistantCommand, "sn Up"));
            normalMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.L }, CommandType.RunUIAssistantCommand, "sn Right"));

            normalMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.F }, CommandType.RunUIAssistantCommand, "hah WidgetsInWindow Click"));
            normalMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.Q }, CommandType.RunUIAssistantCommand, "hah RunningApps Switch"));

            KeybindsInMode.Add(Consts.DefaultMode, normalMode);

            var insertMode = new ModeStorage(true);
            insertMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.OemOpenBrackets }, CommandType.RunEmbeddedCommand, "Cancel"));

            KeybindsInMode.Add("Insert", insertMode);

            PrepareEmacsMode();

            var userMode = new ModeStorage(true);
            KeybindsInMode.Add("Custom1", userMode);

            var userMode2 = new ModeStorage(true);
            KeybindsInMode.Add("Custom2", userMode2);
            var userMode3 = new ModeStorage(true);
            KeybindsInMode.Add("Custom3", userMode3);
            var userMode4 = new ModeStorage(true);
            KeybindsInMode.Add("Custom4", userMode4);
        }

        private void PrepareEmacsMode()
        {
            var emacsMode = new ModeStorage(true);

            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.N }, new[] { Key.Down }));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.P }, new[] { Key.Up }));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.F }, new[] { Key.Right }));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.B }, new[] { Key.Left }));

            emacsMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.F }, new[] { Key.RightCtrl, Key.Right }));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.B }, new[] { Key.RightCtrl, Key.Left }));

            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.V }, new[] { Key.PageDown }));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.V }, new[] { Key.PageUp }));

            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.E }, new[] { Key.End }));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.A }, new[] { Key.Home }));

            emacsMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.LeftShift, Key.OemComma }, new[] { Key.RightCtrl, Key.Home }));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.LeftShift, Key.OemPeriod }, new[] { Key.RightCtrl, Key.End }));

            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.G }, CommandType.RunEmbeddedCommand, "ChangeMode Emacs"));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.Space }, CommandType.RunEmbeddedCommand, "ChangeMode Visual"));

            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.H }, new[] { Key.Back }));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.D }, new[] { Key.Delete }));

            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.W }, CommandType.RunEmbeddedCommand, "EmacsLike kill-region"));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftAlt, Key.W }, CommandType.RunEmbeddedCommand, "EmacsLike kill-ring-save"));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.Y }, CommandType.RunEmbeddedCommand, "EmacsLike yank"));

            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.X }, CommandType.RunEmbeddedCommand, "ChangeMode EmacsC-x"));
            emacsMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.OemQuestion }, new[] { Key.RightCtrl, Key.Z }));

            var emacsCxMode = new ModeStorage(false, true);
            emacsCxMode.Add(new KeyTranslator(new[] { Key.K }, new[] { Key.RightCtrl, Key.F4 }));
            emacsCxMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.C }, new[] { Key.LeftAlt, Key.F4 }));
            emacsCxMode.Add(new KeyTranslator(new[] { Key.LeftCtrl, Key.G }, CommandType.RunEmbeddedCommand, "Cancel"));
            emacsCxMode.Add(new KeyTranslator(new[] { Key.H }, new[] { Key.RightCtrl, Key.A }));
            emacsCxMode.Add(new KeyTranslator(new[] { Key.U }, new[] { Key.RightCtrl, Key.Z }));

            KeybindsInMode.Add("EmacsC-x", emacsCxMode);
            KeybindsInMode.Add("Emacs", emacsMode);
        }
    }
}
