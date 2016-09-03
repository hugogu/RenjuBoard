namespace Renju.Core
{
    using System;
    using System.Linq;
    using AI;
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
            Container.RegisterType(typeof(GameSessionController<>), new ContainerControlledLifetimeManager());
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<BoardRecorder>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGamePlayer, ResolverBasedAIGamePlayer>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGameBoard<IReadOnlyBoardPoint>, GameBoard>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGameBoard<IReadOnlyBoardPoint>, GameBoard>("AI", new ContainerControlledLifetimeManager());
            container.RegisterType<IGameRuleEngine, DefaultGameRuleEngine>();
        }
    }
}
