namespace RenjuBoard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;
    using Properties;
    using Renju.Infrastructure;
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
        }

        private void RegistApplicationDependencies(IUnityContainer container)
        {
            container.RegisterType(typeof(IEnumerable<>), new InjectionFactory((c, type, name) => c.ResolveAll(type.GetGenericArguments().Single())));
            container.RegisterType<GameOptions>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(c => c.BuildUp(new GameOptions().CopyFromObject(Settings.Default))));
            container.RegisterType<LogsViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterInstance<Action<IUnityContainer>>(GetType().Name, RegisterTypes);
            container.RegisterType<GameSessionController>(new ContainerControlledLifetimeManager());
        }
    }
}
