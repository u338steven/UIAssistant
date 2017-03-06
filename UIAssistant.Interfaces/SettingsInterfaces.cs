using System;
using System.Collections.Generic;
using KeybindHelper;

namespace UIAssistant.Interfaces.Settings
{
    public interface IFileIO
    {
        ISettings Read(Type type, params string[] pathParts);
        void Write(Type type, ISettings data, params string[] pathParts);
    }

    public interface ISettings
    {
        void SetValuesDefault();
    }

    public interface IUserSettings : ISettings
    {
        void Save();
        Keybind Back { get; set; }
        Keybind Clear { get; set; }
        List<Keybind> Commands { get; set; }
        string Culture { get; set; }
        Keybind Delete { get; set; }
        HashSet<string> DisabledPlugins { get; set; }
        Keybind Down { get; set; }
        Keybind EmergencySwitch { get; set; }
        Keybind End { get; set; }
        Keybind Execute { get; set; }
        Keybind Home { get; set; }
        int ItemsCountPerPage { get; set; }
        Keybind Left { get; set; }
        string MigemoDictionaryPath { get; set; }
        string MigemoDllPath { get; set; }
        Keybind PageDown { get; set; }
        Keybind PageUp { get; set; }
        Keybind Quit { get; set; }
        Keybind Right { get; set; }
        bool RunAtLogin { get; set; }
        Keybind ShowExtraActions { get; set; }
        Keybind SwitchKeyboardLayout { get; set; }
        Keybind SwitchTheme { get; set; }
        Keybind TemporarilyHide { get; set; }
        string Theme { get; set; }
        Keybind Up { get; set; }
        Keybind Usage { get; set; }
        bool UseMigemo { get; set; }
    }
}

