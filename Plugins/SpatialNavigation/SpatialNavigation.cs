using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;

using UIAssistant.Core.Commands;
using UIAssistant.Utility.Extensions;

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
    [ExportMetadata("Name", "Spatial Navigation")]
    [ExportMetadata("Author", "u338.steven")]
    [ExportMetadata("SupportUri", "https://github.com/u338steven/UIAssistant/")]
    [ExportMetadata("IconUri", "/SpatialNavigation;component/Resources/SpatialNavigation.png")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("CommandName", Consts.Command)]
    public class SpatialNavigation : IPlugin, IDisposable
    {
        public void Initialize()
        {
            var args = new[] { new ArgumentNode(Consts.Up), new ArgumentNode(Consts.Down), new ArgumentNode(Consts.Left), new ArgumentNode(Consts.Right) };
            UIAssistantAPI.RegisterCommand(new CommandNode(Consts.Command, args));
            Navigation.Initialize();
        }

        public Action GenerateAction(IList<string> args)
        {
            var unit = ParseArgs(args);
            if (args.Any(arg => Consts.Up.EqualsWithCaseIgnored(arg)))
            {
                return () => Navigation.MoveTo(Direction.Up, unit);
            }
            else if (args.Any(arg => Consts.Down.EqualsWithCaseIgnored(arg)))
            {
                return () => Navigation.MoveTo(Direction.Down, unit);
            }
            else if (args.Any(arg => Consts.Left.EqualsWithCaseIgnored(arg)))
            {
                return () => Navigation.MoveTo(Direction.Left, unit);
            }
            else if (args.Any(arg => Consts.Right.EqualsWithCaseIgnored(arg)))
            {
                return () => Navigation.MoveTo(Direction.Right, unit);
            }
            return () => Navigation.MoveTo(Direction.Down, unit);
        }

        private Unit ParseArgs(IList<string> args)
        {
            var unit = args.SkipWhile(x => !x.StartsWith("-g", StringComparison.CurrentCultureIgnoreCase));
            if (unit.Count() > 0)
            {
                return Unit.Group;
            }
            return Unit.Item;
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
