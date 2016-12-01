﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIAssistant.Core.Enumerators;
using UIAssistant.Core.Themes;
using UIAssistant.Core.Input;
using UIAssistant.Core.I18n;
using UIAssistant.Utility.Win32;
using UIAssistant.Utility.Extensions;

using UIAssistant.Plugin.HitaHint.Enumerators;
using UIAssistant.Plugin.HitaHint.Operations;

namespace UIAssistant.Plugin.HitaHint
{
    internal class StateController : AbstractStateController
    {
        public Win32Window PreviousWindow { get; set; }
        public EnumerateTarget Target { get; set; }
        public HitaHintSettings Settings { get; private set; }
        private IWidgetEnumerator _enumerator;
        private History _history = new History();
        private StringBuilder _inputText = new StringBuilder();
        private HUDItemCollection _enumeratedResults;

        public StateController()
        {
            Settings = HitaHintSettings.Instance;
            if (Settings.IsMouseCursorHidden)
            {
                Reset();
            }
        }

        public void Reset()
        {
            MouseCursor.AutoHide = Settings.IsMouseCursorHidden;
            MouseCursor.SetCursorVisibility(!Settings.IsMouseCursorHidden);
        }

        public void Initialize()
        {
            _history = new History();
            PreviousWindow = Win32Window.ActiveWindow;
            SubscribeReturnMouseCursor();
            UIAssistantAPI.DefaultHUD.Initialize();
        }

        public void ActivateLastActiveWindow()
        {
            PreviousWindow?.Activate();
        }

        private static bool _noReturnCursor = false;
        private static System.Windows.Point _prevMousePosition;
        private void SubscribeReturnMouseCursor()
        {
            _prevMousePosition = MouseOperation.GetMousePosition();
            Finished += (_, __) =>
            {
                if (!OperationManager.CurrentCommand.IsReturnCursor || _noReturnCursor)
                {
                    return;
                }
                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(300);
                    MouseOperation.Move(_prevMousePosition);
                });
            };
        }

        public override void Quit()
        {
            // NOTE: Why async?
            // In order to process a next keyup event
            Task.Run(() =>
            {
                _enumerator?.Dispose();
                UIAssistantAPI.RemoveDefaultHUD();
                UIAssistantAPI.TopMost = false;
                Cleanup();
                OperationManager.CurrentCommand.Dispose();
            });
        }

        public void SaveState()
        {
            _history.PushState(OperationManager.CurrentName, Target, _enumerator);
        }

        public void Undo()
        {
            if (!_history.CanUndo)
            {
                return;
            }
            var item = _history.PopState();
            if (Win32Window.ActiveWindow != item.window)
            {
                item.window.Activate();
            }
            OperationManager.Change(item.OperationName);
            Target = item.Target;
            _enumerator = item.Enumerator;
        }

        public void ChangeTarget(EnumerateTarget target)
        {
            Target = target;
            _enumerator = Enumerator.Factory(target);
        }

        public void ChangeOperation(string operationName)
        {
            OperationManager.Change(operationName);
        }

        public void ParseArguments(IList<string> args)
        {
            OperationManager.SetDefaultOpration(args);
            SetTheme(args);
            _noReturnCursor = args.Any(x => x.StartsWithCaseIgnored(Consts.NoReturnCursor));
        }

        // TODO: Experimental CommandOption
        public void SetTheme(IList<string> args)
        {
            var theme = args.SkipWhile(x => !x.StartsWithCaseIgnored(Consts.Theme));
            if (theme.Count() > 0)
                _themeSwitcher.Switch(theme.ElementAt(0).Split(':')[1]);
            else
                _themeSwitcher.Switch(Settings.Theme);
        }

        public void Enumerate()
        {
            _enumeratedResults = UIAssistantAPI.DefaultHUD.Items;
            _enumeratedResults.Clear();
            var t = Task.Run(() => _enumerator.Enumerate(_enumeratedResults));
            t.Wait();
            if (_enumeratedResults == null || _enumeratedResults.Count == 0)
            {
                UIAssistantAPI.NotifyInfoMessage("Hit-a-Hint", TextID.NoOneFound.GetLocalizedText());
                Quit();
                return;
            }
            AssignHint(_enumeratedResults);
            UIAssistantAPI.DefaultHUD.Update();
            UIAssistantAPI.TopMost = true;
        }

        string _layoutName;
        public void SetKeyboardLayoutName(string name)
        {
            _layoutName = name;
        }

        private string _messageFormat = "Hit-a-Hint:Input:";
        public void PrintState()
        {
            string cultureName = string.Empty;
            if (!string.IsNullOrEmpty(_layoutName))
            {
                cultureName = $" Keyboard Layout:[{_layoutName}]";
            }
            UIAssistantAPI.DefaultHUD.TextBox.SetText($"{_messageFormat} [ {_inputText.ToString()} ] {OperationManager.CurrentName} {cultureName}");
        }

        public void Invoke(IHUDItem item)
        {
            var center = new System.Windows.Rect(item.Bounds.Center(), new System.Windows.Size(0, 0));
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
            Resumed += (_, __) =>
            {
                _inputText.Clear();
                Enumerate();
                PrintState();
            };
            InvokeAnotherPlugin(command);
        }

        public override void Cleanup()
        {
            _enumerator = null;
            _enumeratedResults = null;
            _inputText?.Clear();
            base.Cleanup();
        }

        private void AssignHint(HUDItemCollection items)
        {
            var hints = UIAssistantAPI.GenerateHints(HitaHintSettings.Instance.HintKeys, items.Count);
            foreach (var x in items.Select((v, i) => new { v, i }))
            {
                x.v.InternalText = hints.ElementAt(x.i);
                x.v.DisplayText = hints.ElementAt(x.i) + x.v.DisplayText;
            }
        }

        public IEnumerable<IHUDItem> FilterInternal(IEnumerable<IHUDItem> list, params string[] inputs)
        {
            return list.Where(item => item.InternalText.StartsWith(inputs[0].ToString())).ToList();
        }

        int _notFoundCount = 0;
        StringBuilder _notFoundInput = new StringBuilder();
        const int MaxNotFoundCount = 3;
        private bool WhetherShowWarning(string inputChar)
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

        public void FilterHints(string inputChar)
        {
            _inputText.Append(inputChar);
            if (!Settings.HintKeys.Contains(inputChar))
            {
                if (WhetherShowWarning(inputChar))
                {
                    UIAssistantAPI.NotifyInfoMessage("Hit-a-Hint", TextID.NoOneFound.GetLocalizedText() + $"\nInput:{_notFoundInput.ToString()}\nKeyboard Layout:{_layoutName}");
                    _notFoundInput.Clear();
                }
                _inputText.Remove(_inputText.Length - 1, 1);
                return;
            }
            _notFoundCount = 0;
            _notFoundInput.Clear();

            var items = _enumeratedResults.Where(item => item.InternalText.StartsWith(_inputText.ToString())).ToList();
            var resultCount = items.Count;

            if (resultCount == 0)
            {
                UIAssistantAPI.NotifyInfoMessage("Hit-a-Hint", TextID.NoOneFound.GetLocalizedText() + $"\nInput:{_inputText}");
                _inputText.Remove(_inputText.Length - 1, 1);
                return;
            }
            else if (resultCount == 1)
            {
                Invoke(items[0]);
                return;
            }
            UIAssistantAPI.DefaultHUD.Items = new HUDItemCollection(items);
            PrintState();
        }

        ThemeSwitcher _themeSwitcher = new ThemeSwitcher();
        public override void SwitchNextTheme()
        {
            _themeSwitcher.Next();
            Settings.Theme = _themeSwitcher.CurrentTheme.FileName;
            UIAssistantAPI.NotifyInfoMessage("Switch Theme", string.Format(TextID.SwitchTheme.GetLocalizedText(), Settings.Theme));
            Settings.Save();
        }

        public void Clear()
        {
            _inputText.Clear();
        }

        public void Back()
        {
            if (_inputText.Length > 0)
            {
                _inputText.Remove(_inputText.Length - 1, 1);
                FilterHints("");
            }
            else
            {
                if (!_history.CanUndo)
                {
                    return;
                }
                Undo();
                Enumerate();
                PrintState();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Settings.IsMouseCursorHidden)
            {
                MouseCursor.SetCursorVisibility(true);
                MouseCursor.DestroyCursor();
            }
            base.Dispose(disposing);
        }
    }
}
