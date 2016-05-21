namespace RenjuBoard
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
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
            LoadResourceFile(key => key.Contains("-" + Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName));
            LoadResourceFile(key => key.StartsWith("views"));
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

        private void LoadResourceFile(Func<string, bool> withKeyThat)
        {
            foreach (var resourceDictionary in Assembly.GetExecutingAssembly().TryFindResourceDictionaries(withKeyThat))
            {
                Trace.WriteLine("Loading resource " + resourceDictionary.Source);
                Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }

        private void OnTaskSchedulerException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Trace.WriteLine(e.Exception.Message);
        }
    }
}
