﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using KeybindHelper;
using KeybindHelper.LowLevel;

using UIAssistant.Interfaces;
using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Input;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    class KeyInputController : IKeyboardPlugin
    {
        private StateController _stateController;
        private KeybindsManiacsSettings _settings;
        private Dictionary<string, KeybindStorage> _keybindsDic { get; set; } = new Dictionary<string, KeybindStorage>();
        private KeybindStorage _currentKeybinds { get; set; } = new KeybindStorage();

        IUIAssistantAPI UIAssistantAPI;
        IKeyboardAPI KeyboardAPI;

        public KeyInputController(IUIAssistantAPI api, StateController controller)
        {
            UIAssistantAPI = api;
            KeyboardAPI = UIAssistantAPI.KeyboardAPI;
            _stateController = controller;
            _settings = KeybindsManiacs.Settings;
        }

        public void Initialize(IKeyboardPluginContext context)
        {
            context.HookHandlers.IgnoreInjected = true;
        }

        public void LoadKeybinds(IKeyboardPluginContext context)
        {
            _keybindsDic.Clear();
            foreach (var keybinds in _settings.KeybindsInMode)
            {
                var defaultKeybinds = context.Keybinds;
                defaultKeybinds = KeyboardAPI.CreateKeybindManager();
                var keybindStorage = new KeybindStorage();
                var oneShotDefined = new Dictionary<KeySet, bool>();
                foreach (var keybind in keybinds.Value.Keybinds)
                {
                    if (keybind.InputKeys == null)
                    {
                        continue;
                    }
                    var keyset = new KeySet(keybind.InputKeys);
                    var canDefineOneShot = keybind.CanDefineOneShot;
                    if (canDefineOneShot)
                    {
                        oneShotDefined.Add(keyset, canDefineOneShot);
                    }
                    switch (keybind.Type)
                    {
                        case CommandType.SwapKey:
                            if (canDefineOneShot)
                            {
                                keybindStorage.OneShotKeybinds.Add(keyset, () =>
                                {
                                    var keys = keybind.OneShot;
                                    if (_stateController.Mode == Mode.Visual && ContainsMovingKey(keys))
                                    {
                                        KeyboardAPI.KeyboardOperation.SendKeys(new[] { Key.RightShift }.Concat(keys).ToArray());
                                        return;
                                    }
                                    KeyboardAPI.KeyboardOperation.SendKeys(keys);
                                }, KeyState.Down, true);
                            }
                            defaultKeybinds.Add(keyset, () =>
                            {
                                var keys = keybind.OutputKeys;
                                var isVisual = _stateController.Mode == Mode.Visual && ContainsMovingKey(keys);
                                if (isVisual)
                                {
                                    keys = new[] { Key.RightShift }.Concat(keys).ToArray();
                                }
                                KeyboardAPI.KeyboardOperation.KeyDown(keys);
                                if (isVisual)
                                {
                                    KeyboardAPI.KeyboardOperation.KeyUp(Key.RightShift);
                                }
                            }, KeyState.Down, true);
                            defaultKeybinds.Add(keyset, () =>
                            {
                                var keys = keybind.OutputKeys;
                                KeyboardAPI.KeyboardOperation.KeyUp(keys);
                            }, KeyState.Up, true);
                            break;
                        case CommandType.RunEmbeddedCommand:
                            defaultKeybinds.Add(keyset, ParseCommand(keybind.CommandText, keybind.InputKeys), KeyState.Down, true);
                            break;
                        case CommandType.RunUIAssistantCommand:
                            defaultKeybinds.Add(keyset, () =>
                            {
                                var command = keybind.CommandText;
                                if (KeybindsManiacs.UIAssistantAPI.PluginManager.Exists(command))
                                {
                                    KeybindsManiacs.UIAssistantAPI.PluginManager.Execute(command);
                                }
                                else
                                {
                                    UIAssistantAPI.NotificationAPI.NotifyWarnMessage("Plugin Error", string.Format(KeybindsManiacs.UIAssistantAPI.LocalizationAPI.Localize(TextID.CommandNotFound), command));
                                }
                            }, KeyState.Down, true);
                            break;
                        //case CommandType.RunExtensionCommand:
                        //    break;
                        default:
                            break;
                    }
                }
                keybindStorage.IsEnabledWindowsKeybinds = keybinds.Value.IsEnabledWindowsKeybinds;
                keybindStorage.IsPrefix = keybinds.Value.IsPrefix;
                keybindStorage.OneShotDefined = oneShotDefined;
                keybindStorage.Keybinds = defaultKeybinds;
                _keybindsDic.Add(keybinds.Key, keybindStorage);
            }
            SwitchMode(_settings.Mode, true);
        }

        private bool ContainsMovingKey(Key[] keys)
        {
            return keys.Any(x => IsMovingKey(x));
        }

        private bool IsMovingKey(Key key)
        {
            if (key == Key.Left || key == Key.Right || key == Key.Down || key == Key.Up || key == Key.PageDown || key == Key.PageUp || key == Key.Home || key == Key.End)
            {
                return true;
            }
            return false;
        }

        private int _startIndex = -1;
        private Action ParseCommand(string command, Key[] keys)
        {
            if (string.IsNullOrEmpty(command))
            {
                return null;
            }
            var commands = command.Split(' ');
            if (commands[0] == "Cancel")
            {
                return () =>
                {
                    if (_command != null)
                    {
                        _command = null;
                        return;
                    }
                    SwitchMode(_settings.Mode);
                };
            }
            else if (commands[0] == "Exit")
            {
                return () =>
                {
                    Toggle();
                };
            }
            else if (commands[0] == "ChangeMode")
            {
                if (commands.Length < 2)
                {
                    return null;
                }
                
                var mode = ModeExtensions.GetMode(commands[1]);
                var modeName = commands[1];
                if (mode == Mode.Unknown)
                {
                    return () =>
                    {
                        _startIndex = -1;
                        if (mode == Mode.Visual)
                        {
                            if (_stateController.Mode == Mode.Visual)
                            {
                                //SwitchMode(Mode.Normal);
                                SwitchMode(_settings.Mode);
                                return;
                            }
                            _startIndex = EmbeddedCommand.GetCaretPos();
                        }
                        SwitchMode(modeName);
                    };
                }
                return () =>
                {
                    _startIndex = -1;
                    if (mode == Mode.Visual)
                    {
                        if (_stateController.Mode == Mode.Visual)
                        {
                            //SwitchMode(Mode.Normal);
                            SwitchMode(_settings.Mode);
                            return;
                        }
                        _startIndex = EmbeddedCommand.GetCaretPos();
                    }
                    SwitchMode(mode);
                };
            }
            else if (commands[0] == "VimLike")
            {
                if (commands.Length < 2)
                {
                    return null;
                }
                if (commands[1] == "f")
                {
                    return () => FindCommand();
                }
                else if (commands[1] == "F")
                {
                    return () => FindCommand(true);
                }
                else if (commands[1] == "t")
                {
                    return () => FindCommand(false, true);
                }
                else if (commands[1] == "T")
                {
                    return () => FindCommand(true, true);
                }
                else if (commands[1] == ";")
                {
                    return () => _repeatFind?.Invoke();
                }
                else if (commands[1] == ",")
                {
                    return () => _reverseFind?.Invoke();
                }
                else if (commands[1] == "y")
                {
                    return () => YankCommand(keys);
                }
            }
            else if(commands[0] == "EmacsLike")
            {
                if (commands.Length < 2)
                {
                    return null;
                }
                if (commands[1] == "yank")
                {
                    return () => EmacsLikeCommand.Yank(_stateController, _settings);
                }
                else if(commands[1] == "kill-ring-save")
                {
                    return () => EmacsLikeCommand.KillRingSave(_stateController, _settings);
                }
                else if (commands[1] == "kill-region")
                {
                    return () => EmacsLikeCommand.KillRegion(_stateController, _settings);
                }
            }
            return null;
        }

        private void YankCommand(Key[] keys)
        {
            if (_stateController.Mode == Mode.Visual)
            {
                KeyboardAPI.KeyboardOperation.SendKeys(Key.RightCtrl, Key.C);
                //SwitchMode(Mode.Normal);
                SwitchMode(_settings.Mode, true);
                return;
            }
            _command = (_, keysState) =>
            {
                if (keysState.Equals(new KeySet(keys)))
                {
                    KeyboardAPI.KeyboardOperation.SendKeys(Key.Home, Key.None, Key.RightShift, Key.End, Key.None, Key.RightCtrl, Key.C);
                    _command = null;
                    return;
                }

                if (_currentKeybinds.Keybinds.Contains(keysState))
                {
                    SwitchMode(Mode.Visual, true);
                    _currentKeybinds.Keybinds.Execute(keysState, false);
                }
                KeyboardAPI.KeyboardOperation.SendKeys(Key.RightCtrl, Key.C);
                if (_stateController.Mode == Mode.Visual)
                {
                    //SwitchMode(Mode.Normal, true);
                    SwitchMode(_settings.Mode, true);
                }
                _command = null;
            };
        }

        private void FindCommand(bool backward = false, bool before = false)
        {
            _command = (x, _) =>
            {
                Task.Run(() => EmbeddedCommand.Find(_startIndex, x, backward, before));
                _repeatFind = () => Task.Run(() => EmbeddedCommand.Find(_startIndex, x, backward, before));
                _reverseFind = () => Task.Run(() => EmbeddedCommand.Find(_startIndex, x, !backward, before));
                _command = null;
            };
        }

        private void SwitchModeCommon(string modeName, bool isSilent = false)
        {
            bool before = _currentKeybinds.IsEnabledWindowsKeybinds;
            if (_keybindsDic.ContainsKey(modeName))
            {
                _currentKeybinds = _keybindsDic.FirstOrDefault(x => x.Key == modeName).Value;
            }
            else
            {
                if (_keybindsDic.ContainsKey(_settings.Mode))
                {
                    _currentKeybinds = _keybindsDic[_settings.Mode];
                }
                else
                {
                    _currentKeybinds = _keybindsDic[Consts.DefaultMode];
                }
            }
            var keybinds = _currentKeybinds.Keybinds;

            if (_currentKeybinds.IsPrefix)
            {
                _command = (_, keysState) =>
                {
                    if (keybinds.Contains(keysState))
                    {
                        keybinds.Execute(keysState, false);
                    }
                    SwitchMode(_settings.Mode, true);
                    _command = null;
                };
            }

            if (before && !_currentKeybinds.IsEnabledWindowsKeybinds)
            {
                if (_currentKeySet.Keys.Any(x => x.IsModifiersKey()))
                {
                    var keys = _currentKeySet.Keys.Where(x => x.IsModifiersKey()).ToArray();
                    KeyboardAPI.KeyboardOperation.KeyUp(keys);
                }
            }
        }

        private void SwitchMode(string modeName, bool isSilent = false)
        {
            SwitchModeCommon(modeName, isSilent);
            _stateController.SwitchMode(modeName, isSilent | _currentKeybinds.IsPrefix);
        }

        private void SwitchMode(Mode mode, bool isSilent = false)
        {
            SwitchModeCommon(mode.ToString(), isSilent);
            _stateController.SwitchMode(mode, isSilent | _currentKeybinds.IsPrefix);
        }

        Action<string, KeySet> _command;
        Action _repeatFind;
        Action _reverseFind;
        bool _isActive = false;
        public void Toggle()
        {
            if (!_isActive)
            {
                _stateController.Initialize();
                var keyController = KeyboardAPI.CreateKeyboardController(this, _stateController.Session);
                UIAssistantAPI.NotificationAPI.NotifyInfoMessage("Keybinds Maniacs", KeybindsManiacs.Localizer.GetLocalizedText(Consts.Activate));
                keyController.Observe();
            }
            else
            {
                UIAssistantAPI.NotificationAPI.NotifyInfoMessage("Keybinds Maniacs", KeybindsManiacs.Localizer.GetLocalizedText(Consts.Deactivate));
                _stateController.Quit();
            }
            _isActive = !_isActive;
        }

        KeySet _currentKeySet = new KeySet();
        public void OnKeyDown(IKeyboardPluginContext context, object sender, LowLevelKeyEventArgs e)
        {
            var keybinds = _currentKeybinds.Keybinds;
            if (e.CurrentKeyInfo.IsInjected)
            {
                e.Handled = false;
                return;
            }
            e.Handled = true;

            var keysState = e.PressedKeys;
            var key = e.CurrentKeyInfo.Key;
            _currentKeySet = keysState;

            if (keysState.Keys.Count == 1)
            {
                if (_currentKeybinds.OneShotDefined.ContainsKey(keysState))
                {
                    keybinds.Execute(keysState, e.CurrentKeyInfo.IsKeyHoldDown);
                    return;
                }
            }

            var input = e.ConvertToCurrentLanguage();
            if (input.Length > 0 && _command != null)
            {
                _command.Invoke(input, keysState);
                return;
            }
            if (keybinds.Contains(keysState))
            {
                if (e.CurrentKeyInfo.IsKeyHoldDown && !keybinds.CanActWhenKeyRepeat(keysState))
                {
                    return;
                }
                var operation = keybinds.GetAction(keysState);
                ReleaseKeys(keysState);
                operation?.Invoke();
                return;
            }

            //var keyset = new KeySet(key);
            var keyset = GenerateKeySet(key, keysState);
            if (keybinds.Contains(keyset))
            {
                if (e.CurrentKeyInfo.IsKeyHoldDown && !keybinds.CanActWhenKeyRepeat(keyset))
                {
                    return;
                }
                var operation = keybinds.GetAction(keyset);
                ReleaseKeys(keysState);
                operation?.Invoke();
                return;
            }

            e.Handled = !_currentKeybinds.IsEnabledWindowsKeybinds;
            return;
        }

        public void OnKeyUp(IKeyboardPluginContext context, object sender, LowLevelKeyEventArgs e)
        {
            var keybinds = _currentKeybinds.Keybinds;
            if (e.CurrentKeyInfo.IsInjected)
            {
                e.Handled = false;
                return;
            }
            e.Handled = true;

            var key = e.CurrentKeyInfo.Key;

            var oldKeysState = new KeySet(key);
            if (_currentKeybinds.OneShotKeybinds.Contains(oldKeysState))
            {
                keybinds.Execute(oldKeysState, e.CurrentKeyInfo.IsKeyHoldDown, KeyState.Up);
                if (e.CurrentKeyInfo.IsOneShot)
                {
                    _currentKeybinds.OneShotKeybinds.Execute(oldKeysState, e.CurrentKeyInfo.IsKeyHoldDown);
                }
                return;
            }

            var keysState = GenerateKeySet(key, e.PressedKeys);
            if (keybinds.Contains(keysState, KeyState.Up))
            {
                var operation = keybinds.GetAction(keysState, KeyState.Up);
                ReleaseKeys(keysState);
                operation?.Invoke();
                return;
            }

            e.Handled = !_currentKeybinds.IsEnabledWindowsKeybinds;
            return;
        }

        private void ReleaseKeys(KeySet keysState)
        {
            if (_currentKeybinds.IsEnabledWindowsKeybinds)
            {
                var pressedKeys = keysState.Keys.Where(x => x.IsModifiersKey()).ToArray();
                KeyboardAPI.KeyboardOperation.Initialize(pressedKeys);
            }
            else
            {
                KeyboardAPI.KeyboardOperation.Initialize();
            }
        }

        private KeySet GenerateKeySet(Key key, KeySet keysState)
        {
            var mod = keysState.Keys.Where(x => x.IsModifiersKey());
            var result = new KeySet(mod.Concat(new[] { key }).ToArray());
            return result;
        }

        public void Cleanup(IKeyboardPluginContext context)
        {
        }

        public void Dispose()
        {
        }
    }
}
