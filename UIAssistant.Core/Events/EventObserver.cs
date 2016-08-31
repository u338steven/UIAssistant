using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Windows.Automation;

namespace UIAssistant.Core.Events
{
    public abstract class EventObserver
    {
        public Action<AutomationElement> Callback { get; set; }
        protected virtual AutomationEvent Event { get; }
        private AutomationEventHandler _eventHandler;
        private AutoResetEvent[] _autoEvents;

        public void Observe()
        {
            try
            {
                _eventHandler = new AutomationEventHandler(OnEvent);
                Automation.AddAutomationEventHandler(Event, AutomationElement.RootElement, TreeScope.Descendants, _eventHandler);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print($"{ex.Message}");
            }
        }

        private void OnEvent(object src, AutomationEventArgs args)
        {
            var element = src as AutomationElement;
            if (element == null)
            {
                return;
            }

            Callback?.Invoke(element);
            _autoEvents[0].Set();
        }

        public bool Wait(int millisecondsTimeout = 3000)
        {
            _autoEvents = new AutoResetEvent[]
            {
                new AutoResetEvent(false)
            };
            return WaitHandle.WaitAll(_autoEvents, millisecondsTimeout, true);
        }

        private void RemoveEventHandler()
        {
            try
            {
                Automation.RemoveAutomationEventHandler(Event, AutomationElement.RootElement, _eventHandler);
            }
            finally
            {

            }
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
                    Callback = null;
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。
                RemoveEventHandler();
                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~EventObserver()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class PopupObserver : EventObserver
    {
        protected override AutomationEvent Event
        {
            get
            {
                return AutomationElement.MenuOpenedEvent;
            }
        }
    }

    public class StructureChangedObserver : EventObserver
    {
        protected override AutomationEvent Event
        {
            get
            {
                return AutomationElement.StructureChangedEvent;
            }
        }
    }

    public class FocusObserver : EventObserver
    {
        protected override AutomationEvent Event
        {
            get
            {
                return AutomationElement.AutomationFocusChangedEvent;
            }
        }
    }
}
