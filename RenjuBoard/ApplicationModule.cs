namespace RenjuBoard
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Prism.Events;
    using Prism.Modularity;
    using Properties;
    using Renju.AI;
    using Renju.Core;
    using Renju.Infrastructure;
    using Renju.Infrastructure.AI;
    using Renju.Infrastructure.AI.Local;
    using Renju.Infrastructure.Events;
    using Renju.Infrastructure.Execution;
    using Renju.Infrastructure.Model;
    using ViewModels;

    public class ApplicationModule : IModule
    {
        private IUnityContainer _currentGameContainer;

        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        [Dependency]
        public IUnityContainer Container { get; set; }

        public void Initialize()
        {
            RegistApplicationDependencies(Container);
            RenewChildContainerForGame(NewGameOptions.Default);
            EventAggregator.GetEvent<StartNewGameEvent>().Subscribe(OnNewGameEvent, ThreadOption.UIThread, true);
        }

        private static void RegistGameSessionDependencies(IUnityContainer container)
        {
            container.RegisterType<IStepController, ExecutionStepController>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGameBoard<IReadOnlyBoardPoint>, GameBoard>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGameBoard<IReadOnlyBoardPoint>, GameBoard>("AI", new ContainerControlledLifetimeManager());
            container.RegisterType<IGameRuleEngine, DefaultGameRuleEngine>();
            container.RegisterType<IDropSelector, WeightedDropSelector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDropResolver, WinRateGameResolver>(new ContainerControlledLifetimeManager());
            container.RegisterType<IBoardMonitor, LocalGameBoardMonitor>(new ContainerControlledLifetimeManager());
            container.RegisterType<IBoardOperator, LocalGameBoardOperator>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGamePlayer, AIGamePlayer>(new ContainerControlledLifetimeManager());
            container.RegisterType<BoardRecorder>(new ContainerControlledLifetimeManager());
            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies().Where(t => t.Name.EndsWith("ViewModel")),
                WithMappings.FromMatchingInterface,
                WithName.Default,
                WithLifetime.ContainerControlled);
            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies().Where(t => typeof(IGameRule).IsAssignableFrom(t)),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                WithLifetime.PerResolve);
        }

        private static void RegistApplicationDependencies(IUnityContainer container)
        {
            container.RegisterType(typeof(IEnumerable<>), new InjectionFactory((c, type, name) => c.ResolveAll(type.GetGenericArguments().Single())));
            container.RegisterType<GameOptions>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(c => c.BuildUp(new GameOptions().CopyFromObject(Settings.Default))));
            container.RegisterType<LogsViewModel>(new ContainerControlledLifetimeManager());
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
    }
}
