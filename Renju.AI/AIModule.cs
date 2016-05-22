namespace Renju.AI
{
    using System;
    using Infrastructure.AI;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;

    public class AIModule : IModule
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        public void Initialize()
        {
            Container.RegisterInstance<Action<IUnityContainer>>(GetType().Name, RegisterTypes);
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IDropSelector, WeightedDropSelector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDropResolver, WinRateGameResolver>(new ContainerControlledLifetimeManager());
        }
    }
}
