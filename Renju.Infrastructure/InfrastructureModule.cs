namespace Renju.Infrastructure
{
    using System;
    using IO;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;

    public class InfrastructureModule : IModule
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        public void Initialize()
        {
            Container.RegisterType<IMessanger<string>>(
                "Console",
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(c => c.BuildUp(new StreamMessanger<string>(Console.OpenStandardInput(), Console.OpenStandardOutput()))));
        }
    }
}
