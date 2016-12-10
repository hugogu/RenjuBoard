namespace RenjuBoard
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;
    using Properties;
    using Reflection4Net.Extensions;
    using Renju.Infrastructure.Model;
    using ViewModels;

    public class ApplicationModule : IModule
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        public void Initialize()
        {
            RegistApplicationDependencies(Container);
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            var vmTypes = AllClasses.FromLoadedAssemblies().Where(t => t.Name.EndsWith("ViewModel") && !container.IsRegistered(t));
            container.RegisterTypes(
                vmTypes,
                WithMappings.FromMatchingInterface,
                WithName.Default,
                WithLifetime.ContainerControlled);
            container.RegisterInstance<Func<Type, Side, GamePlayerSetupViewModel>>((playerType, side) =>
                container.BuildUp(new GamePlayerSetupViewModel(playerType, side)));
            container.RegisterInstance<Func<Type, ResolverOverride[], IGamePlayer>>((playerType, overrides) => {
                Contract.Assert(typeof(IGamePlayer).IsAssignableFrom(playerType));
                var player = container.Resolve(playerType, overrides) as IGamePlayer;
                container.RegisterInstance(player.Side.ToString(), player);
                return player;
            });
        }

        private void RegistApplicationDependencies(IUnityContainer container)
        {
            container.RegisterType(typeof(IEnumerable<>), new InjectionFactory((c, type, name) => c.ResolveAll(type.GetGenericArguments().Single())));
            container.RegisterType<GameOptions>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(c => c.BuildUp(new GameOptions().MapFrom(Settings.Default))));
            container.RegisterType<LogsViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterInstance<Action<IUnityContainer>>(GetType().Name, RegisterTypes);
            container.RegisterInstance<Func<IUnityContainer>>("GameSession", container.CreateChildContainer);
            container.RegisterType<GameSessionController>(new ContainerControlledLifetimeManager());
        }
    }
}
