using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

using UIAssistant.Interfaces;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.HUD;
using UIAssistant.Interfaces.Resource;
using UIAssistant.Interfaces.Session;

using UIAssistant.Plugin.HitaHint.Enumerators;
using UIAssistant.Plugin.HitaHint.Operations;

namespace UIAssistant.Plugin.HitaHint
{
    internal static class StringBuilderExtensions
    {
        internal static void BackSpace(this StringBuilder sb, int times = 1)
        {
            sb.Remove(sb.Length - times, times);
        }
    }

    public static class CancelableTask<T>
    {
        public static Task<T> Run(Func<T> action, CancellationToken token)
        {
            return Task.Run(() => action.Invoke(), token);
        }
    }

    internal class KeyInputValidator
    {
        const int MaxNotFoundCount = 3;
        private static int _notFoundCount = 0;
        private static StringBuilder _notFoundInput = new StringBuilder();

        private static bool WhetherShowWarning(string inputChar)
        {
            _notFoundInput.Append(inputChar);
            ++_notFoundCount;
            if (_notFoundCount > MaxNotFoundCount)
            {
                _notFoundCount = 0;
                return true;
            }
            return false;
        }

        public static bool IsValid(string inputChar, string hintKeys, IUIAssistantAPI api)
        {
            if (!hintKeys.Contains(inputChar))
            {
                if (WhetherShowWarning(inputChar))
                {
                    api.NotificationAPI.NotifyInfoMessage("Hit-a-Hint", api.Localize(TextID.NoOneFound) + $"\nInput:{_notFoundInput.ToString()}");
                    _notFoundInput.Clear();
                }
                return false;
            }
            return true;
        }

        public static void Cleanup()
        {
            _notFoundCount = 0;
            _notFoundInput.Clear();
        }
    }

    internal class Context // persistent
    {
        internal ISwitcher ThemeSwitcher { get; }

        internal Context(IUIAssistantAPI api)
        {
            ThemeSwitcher = api.ThemeAPI.GetThemeSwitcher();
        }
    }

    internal class State : IDisposable
    {
        internal History History { get; } = new History();
        internal IWindow PreviousWindow { get; set; }
        internal ISession Session { get; private set; }
        internal EnumerateTarget Target { get; set; }
        internal StringBuilder InputText { get; } = new StringBuilder();
        internal ICollection<IHUDItem> EnumeratedResults { get; set; }
        internal IWidgetEnumerator Enumerator;
        internal string KeyboardLayoutName { get; set; }

        private IUIAssistantAPI _api;

        internal State(IUIAssistantAPI api)
        {
            _api = api;
            PreviousWindow = api.WindowAPI.ActiveWindow;
            Session = api.SessionAPI.Create();
        }

        internal void ChangeTarget(EnumerateTarget target)
        {
            Target = target;
            Enumerator = Enumerators.Enumerator.Factory(target);
        }

        internal void Save()
        {
            History.PushState(OperationManager.CurrentName, Target, Enumerator);
        }

        internal void Restore()
        {
            if (!History.CanUndo)
            {
                return;
            }
            var item = History.PopState();
            if (_api.WindowAPI.ActiveWindow != item.window)
            {
                item.window.Activate();
            }
            OperationManager.Change(item.OperationName);
            Target = item.Target;
            Enumerator = item.Enumerator;
        }

        public void Dispose()
        {
            Enumerator?.Dispose();
            InputText?.Clear();
            Session.Dispose();
            Enumerator = null;
            EnumeratedResults = null;
        }
    }

    internal class StateController
    {
        internal bool NoReturnCursor { get; set; } = false;
        internal State State { get; set; }
        private IUIAssistantAPI UIAssistantAPI { get; set; }
        private Context _context { get; }

        public StateController(IUIAssistantAPI api)
        {
            UIAssistantAPI = api;
            _context = new Context(api);
            if (HitaHint.Settings.IsMouseCursorHidden)
            {
                Reset();
            }
        }

        public void Reset()
        {
            UIAssistantAPI.MouseAPI.MouseCursor.AutoHide = HitaHint.Settings.IsMouseCursorHidden;
            UIAssistantAPI.MouseAPI.MouseCursor.SetCursorVisibility(!HitaHint.Settings.IsMouseCursorHidden);
        }

        public void Initialize()
        {
            State = new State(UIAssistantAPI);
            UIAssistantAPI.MouseAPI.ReserveToReturnMouseCursor(State.Session, () => OperationManager.CurrentCommand.IsReturnCursor && !NoReturnCursor);
            UIAssistantAPI.DefaultHUD.Initialize();
        }

        public void ActivateLastActiveWindow()
        {
            State.PreviousWindow?.Activate();
        }

        public void Quit()
        {
            // NOTE: Why async?
            // In order to process a next keyup event
            Task.Run(() =>
            {
                //_cancelToken?.Cancel();
                UIAssistantAPI.RemoveDefaultHUD();
                UIAssistantAPI.TopMost = false;
                Cleanup();
                OperationManager.CurrentCommand.Dispose();
            });
        }

        public void SaveState()
        {
            State.Save();
        }

        public void Undo()
        {
            State.Restore();
        }

        public void ChangeTarget(EnumerateTarget target)
        {
            State.ChangeTarget(target);
        }

        public void ChangeOperation(string operationName)
        {
            OperationManager.Change(operationName);
        }

        private string _temporaryTheme = HitaHint.Settings.Theme;
        public void SetTemporaryTheme(string theme)
        {
            _temporaryTheme = theme;
        }

        public void ApplyTheme()
        {
            _context.ThemeSwitcher.Switch(_temporaryTheme);
        }

        private async Task<ICollection<IHUDItem>> EnumerateAsync(ICollection<IHUDItem> container, CancellationTokenSource tokenSource)
        {
            try
            {
                await Task.Run(() => State.Enumerator.Enumerate(container), tokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                UIAssistantAPI.NotificationAPI.NotifyInfoMessage("Hit-a-Hint", UIAssistantAPI.Localize(TextID.Canceled));
                System.Diagnostics.Debug.Print($"{ex.Message}");
            }
            return container;
        }

        public bool IsBusy { get; private set; }

        public async void Enumerate()
        {
            IsBusy = true;
            UIAssistantAPI.DefaultHUD.Items.Clear();

            var cancelToken = new CancellationTokenSource();
            var t = EnumerateAsync(UIAssistantAPI.DefaultHUD.Items, cancelToken);
            State.Session.Finished += (_, __) => { if (!t.IsCompleted) { cancelToken.Cancel(); } };
            State.EnumeratedResults = await t;
            IsBusy = false;

            if (cancelToken.IsCancellationRequested)
            {
                return;
            }
            if (State.EnumeratedResults.Count == 0)
            {
                UIAssistantAPI.NotificationAPI.NotifyInfoMessage("Hit-a-Hint", UIAssistantAPI.Localize(TextID.NoOneFound));
                Quit();
                return;
            }
            AssignHint(State.EnumeratedResults);
            UIAssistantAPI.DefaultHUD.Update();
            UIAssistantAPI.TopMost = true;
        }

        private string _messageFormat = "Hit-a-Hint:Input:";
        public void PrintState()
        {
            string cultureName = string.Empty;
            if (!string.IsNullOrEmpty(State.KeyboardLayoutName))
            {
                cultureName = $" Keyboard Layout:[{State.KeyboardLayoutName}]";
            }
            UIAssistantAPI.DefaultHUD.TextBox.SetText($"{_messageFormat} [ {State.InputText.ToString()} ] {OperationManager.CurrentName} {cultureName}");
        }

        public void Invoke(IHUDItem item)
        {
            var center = new Rect(item.Bounds.Center(), new Size(0, 0));
            UIAssistantAPI.ScaleIndicatorAnimation(center, item.Bounds, false, 250);
            OperationManager.CurrentCommand.Execute(item);

            if (OperationManager.CurrentCommand.IsContinuous)
            {
                OperationManager.CurrentCommand.Next(this);
                return;
            }
            Quit();
        }

        public void InvokePlugin(string command)
        {
            UIAssistantAPI.CommandAPI.InvokePluginCommand(command, Quit, State.Session.Pause, () =>
            {
                State.Session.Resume();
                State.InputText.Clear();
                Enumerate();
                PrintState();
            });
        }

        public void Cleanup()
        {
            State.Dispose();
            State = null;
        }

        private void AssignHint(ICollection<IHUDItem> items)
        {
            var hints = UIAssistantAPI.GenerateHints(HitaHint.Settings.HintKeys, items.Count);
            foreach (var x in items.Select((v, i) => new { v, i }))
            {
                x.v.InternalText = hints.ElementAt(x.i);
                x.v.DisplayText = hints.ElementAt(x.i) + x.v.DisplayText;
            }
        }

        public void FilterHints(string inputChar)
        {
            State.InputText.Append(inputChar);
            if (!KeyInputValidator.IsValid(inputChar, HitaHint.Settings.HintKeys, UIAssistantAPI))
            {
                State.InputText.BackSpace();
                return;
            }
            KeyInputValidator.Cleanup();

            var items = State.EnumeratedResults.Where(item => item.InternalText.StartsWith(State.InputText.ToString())).ToList();
            var resultCount = items.Count;

            if (resultCount == 0)
            {
                UIAssistantAPI.NotificationAPI.NotifyInfoMessage("Hit-a-Hint", UIAssistantAPI.Localize(TextID.NoOneFound) + $"\nInput:{State.InputText}");
                State.InputText.BackSpace();
                return;
            }
            else if (resultCount == 1)
            {
                Invoke(items[0]);
                return;
            }
            UIAssistantAPI.DefaultHUD.Items = items;
            //UIAssistantAPI.DefaultHUD.Items = new HUDItemCollection(items);
            PrintState();
        }

        public void SwitchNextTheme()
        {
            _context.ThemeSwitcher.Next();
            HitaHint.Settings.Theme = _context.ThemeSwitcher.CurrentTheme.Id;
            UIAssistantAPI.NotificationAPI.NotifyInfoMessage("Switch Theme", string.Format(UIAssistantAPI.Localize(TextID.SwitchTheme), HitaHint.Settings.Theme));
            HitaHint.Settings.Save();
        }

        public void Clear()
        {
            State.InputText.Clear();
        }

        public void Back()
        {
            if (State.InputText.Length > 0)
            {
                State.InputText.BackSpace();
                FilterHints("");
            }
            else
            {
                if (!State.History.CanUndo)
                {
                    return;
                }
                Undo();
                Enumerate();
                PrintState();
            }
        }

        public void Dispose()
        {
            if (HitaHint.Settings.IsMouseCursorHidden)
            {
                HitaHint.UIAssistantAPI.MouseAPI.MouseCursor.SetCursorVisibility(true);
                HitaHint.UIAssistantAPI.MouseAPI.MouseCursor.DestroyCursor();
            }
        }
    }
}
