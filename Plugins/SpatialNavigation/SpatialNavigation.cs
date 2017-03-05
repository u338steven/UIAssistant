using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;

using UIAssistant.Infrastructure.Commands;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Commands;
using UIAssistant.Interfaces.Plugin;

namespace UIAssistant.Plugin.SpatialNavigation
{
    [Flags]
    public enum Unit
    {
        Item,
        Group,
    }

    [Flags]
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    [Export(typeof(IPlugin))]
    [Export(typeof(IDisposable))]
    [ExportMetadata("Guid", "426f2567-b37e-4cf1-8c69-ea27850a67b4")]
    [ExportMetadata("Name", Consts.PluginName)]
    [ExportMetadata("Author", "u338.steven")]
    [ExportMetadata("SupportUri", "https://github.com/u338steven/UIAssistant/")]
    [ExportMetadata("IconUri", "/SpatialNavigation;component/Resources/SpatialNavigation.png")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("CommandName", Consts.Command)]
    public class SpatialNavigation : IPlugin, IDisposable
    {
        private Unit _current { get; set; }
        internal static IUIAssistantAPI UIAssistantAPI { get; private set; }

        public void Initialize(IUIAssistantAPI api)
        {
            UIAssistantAPI = api;
            var args = new[] {
                new ArgumentRule(Consts.Up, Up),
                new ArgumentRule(Consts.Down, Down),
                new ArgumentRule(Consts.Left, Left),
                new ArgumentRule(Consts.Right, Right) };

            var group = new ArgumentRule("-g", x => _current = Unit.Group);
            UIAssistantAPI.RegisterCommand(new CommandRule(Consts.Command, Run, args) { Description = Consts.PluginName });
            Navigation.Initialize();
        }

        public void Setup()
        {
            _current = Unit.Item;
        }

        private void Run(ICommand command)
        {
        }

        private void Up(ICommand command)
        {
            Navigation.MoveTo(Direction.Up, _current);
        }

        private void Down(ICommand command)
        {
            Navigation.MoveTo(Direction.Down, _current);
        }

        private void Left(ICommand command)
        {
            Navigation.MoveTo(Direction.Left, _current);
        }

        private void Right(ICommand command)
        {
            Navigation.MoveTo(Direction.Right, _current);
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
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~SpatialNavigation() {
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
