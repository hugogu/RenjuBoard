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
        private IUnityContainer _currentGameContainer;

        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        [Dependency]
        public IUnityContainer Container { get; set; }

        public void Initialize()
        {
            Application.Current.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            EventAggregator.GetEvent<StartNewGameEvent>().Subscribe(OnNewGameEvent, ThreadOption.UIThread, true);
        }

        private void OnNewGameEvent(NewGameOptions options)
        {
            if (_currentGameContainer != null)
                _currentGameContainer.Dispose();
            _currentGameContainer = Container.CreateChildContainer();
            App.Current.MainWindow.DataContext = _currentGameContainer.Resolve<MainWindowViewModel>(new ParameterOverride("options", options));
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
