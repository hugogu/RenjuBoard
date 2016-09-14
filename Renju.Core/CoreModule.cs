namespace Renju.Core
{
    using System;
    using System.Linq;
    using Infrastructure.Model;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;

    public class CoreModule : IModule
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        public void Initialize()
        {
            Container.RegisterTypes(
                AllClasses.FromLoadedAssemblies().Where(t => typeof(IGameRule).IsAssignableFrom(t)),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                WithLifetime.PerResolve);
            Container.RegisterInstance<Action<IUnityContainer>>(GetType().Name, RegisterTypes);
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies().Where(t => typeof(IGamePlayer).IsAssignableFrom(t)),
                WithMappings.None,
                WithName.Default,
                WithLifetime.ContainerControlled);

            container.RegisterType<BoardRecorder>(new ContainerControlledLifetimeManager());
            // TODO: Investigate why this registration is nessesary and why only one GameBoard is created. (Expected.)
            container.RegisterType<IReadBoardState<IReadOnlyBoardPoint>, GameBoard>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGameBoard<IReadOnlyBoardPoint>, GameBoard>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGameBoard<IReadOnlyBoardPoint>, GameBoard>("AI", new ContainerControlledLifetimeManager());
            container.RegisterType<IGameRuleEngine, DefaultGameRuleEngine>();
        }
    }
}
