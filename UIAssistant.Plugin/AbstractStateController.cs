using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Interfaces.API;

namespace UIAssistant.Plugin
{
    public abstract class AbstractStateController : IDisposable
    {
        public event EventHandler Finished;
        public event EventHandler Pausing;
        public event EventHandler Resumed;

        public abstract void SwitchNextTheme();
        protected IUIAssistantAPI UIAssistantAPI { get; private set; }

        public AbstractStateController(IUIAssistantAPI api)
        {
            UIAssistantAPI = api;
        }

        public virtual void Cleanup()
        {
            Finished?.Invoke(this, EventArgs.Empty);
            Finished = null;
            Pausing = null;
            Resumed = null;
        }

        public void InvokeAnotherPlugin(string command)
        {
            // TODO: Experiment: Another plugin execution
            if (UIAssistantAPI.PluginManager.Exists(command))
            {
                Pausing?.Invoke(this, EventArgs.Empty);
                UIAssistantAPI.DefaultHUD.Initialize();
                UIAssistantAPI.PluginManager.Execute(command);
                UIAssistantAPI.PluginManager.Resume += () =>
                {
                    Resumed?.Invoke(this, EventArgs.Empty);
                };
                UIAssistantAPI.PluginManager.Quit += () =>
                {
                    Quit();
                };
            }
            else
            {
                // TODO:
                //UIAssistantAPI.NotifyWarnMessage("Plugin Error", string.Format(TextID.CommandNotFound.GetLocalizedText(), command));
                Quit();
            }
        }

        public abstract void Quit();

        public void SwitchHUD()
        {
            if (!UIAssistantAPI.IsContextAvailable)
            {
                return;
            }

            if (!UIAssistantAPI.IsContextVisible)
            {
                OnSwitchingToContext(UIAssistantAPI.DefaultHUD.SelectedItem != null);
            }

            UIAssistantAPI.SwitchHUD();
        }

        protected virtual void OnSwitchingToContext(bool isItemSelected)
        {

        }

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
