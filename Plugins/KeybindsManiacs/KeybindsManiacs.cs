using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Plugin;
using UIAssistant.Interfaces.Resource;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    [Export(typeof(IPlugin))]
    [Export(typeof(IConfigurablePlugin))]
    [Export(typeof(ILocalizablePlugin))]
    [Export(typeof(IDisposable))]
    [ExportMetadata("Guid", "a9f485e8-9b42-47fa-90c3-95c40432da06")]
    [ExportMetadata("Name", Consts.PluginName)]
    [ExportMetadata("Author", "u338.steven")]
    [ExportMetadata("SupportUri", "https://github.com/u338steven/UIAssistant/")]
    [ExportMetadata("IconUri", "/KeybindsManiacs;component/Resources/KeybindsManiacs.png")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("CommandName", Consts.Command)]
    public class KeybindsManiacs : IPlugin, IConfigurablePlugin, ILocalizablePlugin, IDisposable
    {
        internal static IUIAssistantAPI UIAssistantAPI { get; private set; }
        private StateController _stateController;
        private KeyInputController _keyController;
        internal static ILocalizer Localizer { get; private set; }
        internal static KeybindsManiacsSettings Settings { get; private set; }

        public void Initialize(IUIAssistantAPI api)
        {
            UIAssistantAPI = api;

            Settings = KeybindsManiacsSettings.Load();
            _stateController = new StateController(api);
            _keyController = new KeyInputController(api, _stateController);
            Localizer = api.GetLocalizer();
            RegisterCommand();

            if (_stateController.Settings.RunAtStartup)
            {
                _keyController.Toggle();
            }
        }

        private void RegisterCommand()
        {
            var command = UIAssistantAPI.CommandAPI.CreateCommandRule(Consts.Command, _ => _keyController.Toggle());
            command.Description = Consts.PluginName;
            UIAssistantAPI.CommandAPI.RegisterCommand(command);
        }

        public void Setup()
        {

        }

        public System.Windows.FrameworkElement GetConfigrationInterface()
        {
            return new Settings();
        }

        public void Localize()
        {
            Localizer.SwitchLanguage(UIAssistantAPI.CurrentLanguage);
        }

        public void Save()
        {
            Settings.Save();
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
