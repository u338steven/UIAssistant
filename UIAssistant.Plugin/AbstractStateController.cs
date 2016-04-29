using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.I18n;

namespace UIAssistant.Plugin
{
    public abstract class AbstractStateController : IDisposable
    {
        public event Action Finished;
        public event Action Pausing;
        public event Action Resumed;

        public abstract void SwitchNextTheme();
        public virtual void Cleanup()
        {
            Finished?.Invoke();
            Finished = null;
            Pausing = null;
            Resumed = null;
        }

        public void InvokeAnotherPlugin(string command)
        {
            // TODO: Experiment: Another plugin execution
            if (PluginManager.Instance.Exists(command))
            {
                Pausing?.Invoke();
                UIAssistantAPI.DefaultHUD.Initialize();
                PluginManager.Instance.Execute(command);
                PluginManager.Instance.Resume += () =>
                {
                    Resumed?.Invoke();
                };
                PluginManager.Instance.Quit += () =>
                {
                    Quit();
                };
            }
            else
            {
                UIAssistantAPI.NotifyWarnMessage("Plugin Error", string.Format(TextID.CommandNotFound.GetLocalizedText(), command));
                Quit();
            }
        }

        public abstract void Quit();

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Cleanup();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
