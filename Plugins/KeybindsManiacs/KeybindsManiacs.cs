using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;
using UIAssistant.Core.Commands;
using UIAssistant.Core.I18n;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    [Export(typeof(IPlugin))]
    [Export(typeof(IConfigurablePlugin))]
    [Export(typeof(ILocalizablePlugin))]
    [Export(typeof(IDisposable))]
    [ExportMetadata("Guid", "a9f485e8-9b42-47fa-90c3-95c40432da06")]
    [ExportMetadata("Name", "Keybinds Maniacs")]
    [ExportMetadata("Author", "u338.steven")]
    [ExportMetadata("SupportUri", "https://github.com/u338steven/UIAssistant/")]
    [ExportMetadata("IconUri", "/KeybindsManiacs;component/Resources/KeybindsManiacs.png")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("CommandName", Consts.Command)]
    public class KeybindsManiacs : IPlugin, IConfigurablePlugin, ILocalizablePlugin, IDisposable
    {
        private StateController _stateController;
        private KeyInputController _keyController;

        public void Initialize()
        {
            _stateController = new StateController();
            _keyController = new KeyInputController(_stateController);
            RegisterCommand();

            if (_stateController.Settings.RunAtStartup)
            {
                _keyController.Toggle();
            }
        }

        private void RegisterCommand()
        {
            var command = new CommandNode(Consts.Command);

            UIAssistantAPI.RegisterCommand(command);
        }

        public Action GenerateAction(IList<string> args)
        {
            return () =>
            {
                //_stateController.Initialize();
                //_keyController.Initialize();
                _keyController.Toggle();
            };
        }

        public System.Windows.FrameworkElement GetConfigrationInterface()
        {
            return new Settings();
        }

        public static Localizer Localizer = new Localizer();
        public void Localize()
        {
            Localizer.SwitchLanguage(DefaultLocalizer.CurrentLanguage);
        }

        public void Save()
        {
            KeybindsManiacsSettings.Instance.Save();
            _keyController?.Reset();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    _keyController?.Dispose();
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~KeybindsManiacs() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
