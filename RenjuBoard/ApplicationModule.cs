using System;
using System.Collections.Generic;
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
            RegistApplicationDependencies(Container);
            RenewChildContainerForGame(NewGameOptions.Default);
            EventAggregator.GetEvent<StartNewGameEvent>().Subscribe(OnNewGameEvent, ThreadOption.UIThread, true);
        }

        private void OnNewGameEvent(NewGameOptions options)
        {
            RenewChildContainerForGame(options);
            App.Current.MainWindow.DataContext = _currentGameContainer.Resolve<MainWindowViewModel>();
        }

        private void RenewChildContainerForGame(NewGameOptions options)
        {
            if (_currentGameContainer != null)
                _currentGameContainer.Dispose();
            _currentGameContainer = Container.CreateChildContainer();
            RegistGameSessionDependencies(_currentGameContainer);
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(_currentGameContainer));
            if (options == NewGameOptions.Default)
                options = _currentGameContainer.Resolve<NewGameOptions>();
            _currentGameContainer.RegisterInstance(options);
            _currentGameContainer.RegisterInstance(BoardPoint.CreateIndexBasedFactory(options.BoardSize));
            var ai = _currentGameContainer.Resolve<IDropResolver>();
            _currentGameContainer.RegisterInstance<IReportExecutionStatus>("ai", ai);
        }

        private static void RegistGameSessionDependencies(IUnityContainer container)
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

        private static void RegistApplicationDependencies(IUnityContainer container)
        {
            container.RegisterType(typeof(IEnumerable<>), new InjectionFactory((c, type, name) => c.ResolveAll(type.GetGenericArguments().Single())));
            container.RegisterType<GameOptions>(new ContainerControlledLifetimeManager());
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var error = String.Format("An unhnalded exception was thrown, do you want to keep RenJu running? \r\n\r\n {0}", e.Exception.Message);
            if (MessageBox.Show(error, "Rejun Exception", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Handled = true;
            }
        }
    }
}
