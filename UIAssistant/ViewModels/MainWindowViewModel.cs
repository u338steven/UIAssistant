using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using System.Windows;
using System.Windows.Interop;

using UIAssistant.Models;
using UIAssistant.Views;
using UIAssistant.Core.API;
using UIAssistant.Core.I18n;
using UIAssistant.Core.Plugin;
using UIAssistant.Interfaces.Native;
using UIAssistant.UI.Controls;

namespace UIAssistant.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public HUDPanel DefaultHUDPanel => new HUDPanel();
        public HUDPanel DefaultContextPanel => new HUDPanel();

        #region Left変更通知プロパティ
        private double _Left;

        public double Left
        {
            get
            { return _Left; }
            set
            { 
                if (_Left == value)
                    return;
                _Left = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Top変更通知プロパティ
        private double _Top;

        public double Top
        {
            get
            { return _Top; }
            set
            { 
                if (_Top == value)
                    return;
                _Top = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Width変更通知プロパティ
        private double _Width;

        public double Width
        {
            get
            { return _Width; }
            set
            { 
                if (_Width == value)
                    return;
                _Width = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Height変更通知プロパティ
        private double _Height;

        public double Height
        {
            get
            { return _Height; }
            set
            { 
                if (_Height == value)
                    return;
                _Height = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public void Initialize()
        {
            Notification.Initialize();
            UIAssistantAPI.Instance.Initialize(DefaultHUDPanel, DefaultContextPanel);

            IntPtr windowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            NativeMethods.SetWindowExTransparent(windowHandle);

            DefaultLocalizer.SwitchLanguage(DefaultLocalizer.FindLanguage(UIAssistantAPI.Instance.UIAssistantSettings.Culture));
            PluginManager.Instance.Localize();

            SettingsWindowModel.RegisterHotkeys();

            MainWindowModel.ShowNotifyIcon();

            UIAssistantAPI.Instance.ThemeAPI.SwitchTheme("General");

            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            UIAssistantAPI.Instance.ViewAPI.TopMost = false;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
        }

        protected override void Dispose(bool disposing)
        {
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
            PluginManager.Instance.Dispose();
            MainWindowModel.HideNotifyIcon();
            System.Threading.Thread.Sleep(500);
            base.Dispose(disposing);
        }
    }
}
