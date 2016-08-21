namespace RenjuBoard
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Renju.Infrastructure;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerException;
            Resources.LoadResourceFileThat(fileName => fileName.Contains("-" + Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName));
            Resources.LoadResourceFileThat(fileName => fileName.StartsWith("views"));
            new RenjuBoardBootstrapper().Run();
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var error = String.Format("An unhnalded exception was thrown, do you want to keep RenJu running? \r\n\r\n {0}", e.Exception.Message);
            if (MessageBox.Show(error, "Rejun Exception", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Handled = true;
            }
        }

        private void OnTaskSchedulerException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Trace.WriteLine(e.Exception.Message);
        }
    }
}
