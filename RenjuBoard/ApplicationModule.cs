using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;
using Renju.AI;
using Renju.AI.Resolving;
using Renju.AI.Weights;
using Renju.Core;
using Renju.Infrastructure.Events;
using Renju.Infrastructure.Execution;
using Renju.Infrastructure.Model;
using RenjuBoard.ViewModels;

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
            App.Current.MainWindow.DataContext = CreateChildContainerForGame(options).Resolve<MainWindowViewModel>();
        }

        private IUnityContainer CreateChildContainerForGame(NewGameOptions options)
        {
            if (_currentGameContainer != null)
                _currentGameContainer.Dispose();
            _currentGameContainer = Container.CreateChildContainer();
            RegistApplicationDependencies(_currentGameContainer);
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(_currentGameContainer));
            if (options == NewGameOptions.Default)
                options = _currentGameContainer.Resolve<NewGameOptions>();
            _currentGameContainer.RegisterInstance(options);
            _currentGameContainer.RegisterInstance(BoardPoint.CreateIndexBasedFactory(options.BoardSize));
            var ai = _currentGameContainer.Resolve<IDropResolver>();
            _currentGameContainer.RegisterInstance<IReportExecutionStatus>("ai", ai);

            return _currentGameContainer;
        }

        private void RegistApplicationDependencies(IUnityContainer container)
        {
            container.RegisterType<IStepController, ExecutionStepController>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGameBoard<IReadOnlyBoardPoint>, GameBoard>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGameRuleEngine, DefaultGameRuleEngine>();
            container.RegisterType<IDropSelector, WeightedDropSelector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDropResolver, WinRateGameResolver>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGamePlayer, AIGamePlayer>(new ContainerControlledLifetimeManager());
            container.RegisterType<OptionsViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterTypes(AllClasses.FromLoadedAssemblies().Where(t => typeof(IGameRule).IsAssignableFrom(t)),
                                    WithMappings.FromAllInterfaces,
                                    WithName.TypeName,
                                    WithLifetime.PerResolve);
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
