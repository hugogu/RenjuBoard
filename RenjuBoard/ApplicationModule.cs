using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;
using Renju.Infrastructure.Events;

namespace RenjuBoard
{
    public class ApplicationModule : IModule
    {
        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        [Dependency]
        public IUnityContainer Container { get; set; }

        public void Initialize()
        {
            Application.Current.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            Container.RegisterType<MainWindowViewModel>(new PerResolveLifetimeManager());
            EventAggregator.GetEvent<StartNewGameEvent>().Subscribe(OnNewGameEvent, ThreadOption.UIThread, true);
        }

        private void OnNewGameEvent(NewGameOptions options)
        {
            Application.Current.MainWindow.SetValue(FrameworkElement.DataContextProperty,
                Container.Resolve<MainWindowViewModel>(new ParameterOverride("options", options)));
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var error = String.Format("An unhnalded exception was thrown, do you want to keep RenJu running? \r\n\r\n {0}", e.Exception.Message);
            if (MessageBox.Show(error, "Rejun Exception", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Handled = true;
            }
        }
    }
}
