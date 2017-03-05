using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Livet;
using UIAssistant.Core.Plugin;
using UIAssistant.Infrastructure.Logger;

using Microsoft.Shell;
using System.Reflection;
using System.Runtime.InteropServices;

namespace UIAssistant
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        [STAThread]
        public static void Main()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            var guid = attribute.Value;
            if (SingleInstance<App>.InitializeAsFirstInstance(guid))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DispatcherHelper.UIDispatcher = Dispatcher;
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
        }

        private void HandleUnhandledException(Exception ex)
        {
            string message = "An unhandled error occured\nYou can copy this message by Ctrl+C\n\n";
            while (ex != null)
            {
                message += ex.Message + "\n";
                message += ex.StackTrace + "\n";
                ex = ex.InnerException;
            }

            try
            {
                PluginManager.Instance.Dispose();
                Models.MainWindowModel.HideNotifyIcon();
            }
            catch
            {

            }
            MessageBox.Show(
                message,
                "Fatal error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            try
            {
                Log.Fatal(ex);
            }
            catch
            {

            }
            Environment.Exit(1);
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException(e.ExceptionObject as Exception);
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }
    }
}
