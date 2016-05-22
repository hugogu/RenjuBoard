namespace RenjuBoard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Prism.Events;
    using Prism.Modularity;
    using Properties;
    using Renju.Core;
    using Renju.Infrastructure;
    using Renju.Infrastructure.AI;
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
            EventAggregator.GetEvent<StartNewGameEvent>().Subscribe(OnNewGameEvent, ThreadOption.UIThread, true);
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            var vmTypes = AllClasses.FromLoadedAssemblies().Where(t => t.Name.EndsWith("ViewModel") && !container.IsRegistered(t));
            container.RegisterTypes(
                vmTypes,
                WithMappings.FromMatchingInterface,
                WithName.Default,
                WithLifetime.ContainerControlled);
        }

        private void RegistGameSessionDependencies(IUnityContainer container)
        {
            foreach(var containerRegister in Container.ResolveAll<Action<IUnityContainer>>())
            {
                containerRegister(container);
            }
        }

        private void RegistApplicationDependencies(IUnityContainer container)
        {
            container.RegisterType(typeof(IEnumerable<>), new InjectionFactory((c, type, name) => c.ResolveAll(type.GetGenericArguments().Single())));
            container.RegisterType<GameOptions>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(c => c.BuildUp(new GameOptions().CopyFromObject(Settings.Default))));
            container.RegisterType<LogsViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterInstance<Action<IUnityContainer>>(GetType().Name, RegisterTypes);
        }

        private void RenewChildContainerForGame(NewGameOptions options)
        {
            if (_currentGameContainer != null)
                _currentGameContainer.Dispose();
            _currentGameContainer = Container.CreateChildContainer();
            RegistGameSessionDependencies(_currentGameContainer);
            if (options == NewGameOptions.Default)
                options = _currentGameContainer.Resolve<NewGameOptions>();
            _currentGameContainer.RegisterInstance(options);
            _currentGameContainer.RegisterInstance(BoardPoint.CreateIndexBasedFactory(options.BoardSize));
            var ai = _currentGameContainer.Resolve<IDropResolver>();
            _currentGameContainer.RegisterInstance<IReportExecutionStatus>("ai", ai);
        }

        private void OnNewGameEvent(NewGameOptions options)
        {
            RenewChildContainerForGame(options);
            App.Current.MainWindow.DataContext = _currentGameContainer.Resolve<MainWindowViewModel>();
        }
    }
}
