# UIAssistant

(This readme is written in English and Japanese mixed. 藪からスティックに日本語がトゥギャザーしてます。)

UIAssistant is a Windows tool for supporting your UI a little.

Features ex.

- Hit-a-Hint: You can click quickly a button in most window with keyboard.
- Search by Text: You can search a window for a string by input text.

New Feature(ver.0.2)

- [Spatial Navigation](https://github.com/u338steven/UIAssistant/wiki/Spatial-Navigation)

UIAssistant is under the MIT license.

<!--See https://github.com/u338steven/UIAssistant/wiki for details.-->

![Introduction](https://raw.github.com/wiki/u338steven/UIAssistant/images/demo/introduction.gif)

## Features

- Support plugin
- [Customizable Theme(WPF)](https://github.com/u338steven/UIAssistant/wiki/Customizable-Theme), Keybinds
- [Multi-Languages](https://github.com/u338steven/UIAssistant/wiki/Multi-Languages)
- Hotkey-Command-Driven
- Do not steal focus from current window
- Support Migemo, cannot input from IME(なので日本語入力ができません(ただし開発者は日本人))

## Requirements

- Windows7 SP1 or later
- .NETFramework 4.6

## Quick start

### Installation

You can download from [here(Releases)](https://github.com/u338steven/UIAssistant/releases/) (include Migemo ja-JP, zh-CN and zh-TW Dictionaries)  
Uncompress it and run UIAssistant.exe

**Administrator permission are recommended for running UIAssistant.**  
Otherwise, in some cases, UIAssistant cannot enumerate widgets in a window.

### Hit-a-Hint

#### Hotkeys

##### [Enumerate widgets in current window](https://github.com/u338steven/UIAssistant/wiki/Enumerate-widgets-in-current-window)

Default keybind: Ctrl+Alt+E

##### [Enumerate widgets in taskbar](https://github.com/u338steven/UIAssistant/wiki/Enumerate-widgets-in-taskbar)

Default keybind: Ctrl+Alt+R

##### [Enumerate divided screen elements](https://github.com/u338steven/UIAssistant/wiki/Enumerate-divided-screen-elements)

Default keybind: Ctrl+Alt+G

##### [Enumerate running apps](https://github.com/u338steven/UIAssistant/wiki/Enumerate-running-apps)

Default keybind: Ctrl+Alt+Q

#### Keybinds

##### [Show/Hide usage](https://github.com/u338steven/UIAssistant/wiki/Show-Hide-usage)

Default keybind: LeftCtrl+U

##### Quit

Default keybind: Escape

### Search by Text

#### Hotkeys

##### [Search strings in current window](https://github.com/u338steven/UIAssistant/wiki/Search-strings-in-current-window)

Default keybind: Ctrl+Alt+T

##### [Search strings in focused container](https://github.com/u338steven/UIAssistant/wiki/Search-strings-in-focused-container)

Default keybind: Ctrl+Alt+L  
container: ComboBox, ListBox or ListView

##### [Search commands in current window](https://github.com/u338steven/UIAssistant/wiki/Search-commands-in-current-window)

Default keybind: Ctrl+Alt+K  
commands: RibbonUI, Menu or Toolbar items

#### Keybinds

##### [Show/Hide usage](https://github.com/u338steven/UIAssistant/wiki/Show-Hide-usage)

Default keybind: LeftCtrl+U

##### Quit

Default keybind: Escape

### Settings

Rightclick Tasktray icon -> Settings

## Known issues

- Cannot almost enumerate widgets of Explorer.exe
- Cannot almost enumerate widgets of some web browsers (e.g. Google Chrome, Microsoft Edge)
- Few validations(Settings window may be a pandemonium)  
↓He says, "Hi!"  
![Hi!](https://raw.github.com/wiki/u338steven/UIAssistant/images/hi.png)

## Report bugs

Please provide the information below:

- UIAssistant version or commit number
- Windows version
- The steps to reproduce the bug

if you have:

- Exception information
- Logs from UIAssistant\logs
- Screenshot

↓He says, "*We* will be back..."  
![We will be back...](https://raw.github.com/wiki/u338steven/UIAssistant/images/wewillbeback.png)

## Planned features

- Auto-Updater
~~- Spatial Navigation~~ Added in ver.0.2
- Vim-like keybinds on Windows
- Command direct input

## TODO

- Write wiki for details
- Refactor, Refactor, Refactor

## Acknowledgements

This project is influenced by a number of other projects. Notably:

- [Keysnail](https://github.com/mooz/keysnail)
- [Wox](https://github.com/Wox-launcher/Wox)

## License

This project is licensed under the MIT license, Copyright (c) 2016 u338.steven.
