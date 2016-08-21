namespace Renju.Infrastructure
{
    using System;
    using AI;
    using AI.Local;
    using Execution;
    using IO;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;

    public class InfrastructureModule : IModule
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        public void Initialize()
        {
            Container.RegisterInstance<Action<IUnityContainer>>(GetType().Name, RegisterTypes);
            Container.RegisterType<IMessanger<string, string>>(
                "Console",
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(c => c.BuildUp(new StreamMessanger<string, string>(Console.OpenStandardInput(), Console.OpenStandardOutput()))));
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IStepController, ExecutionStepController>(new ContainerControlledLifetimeManager());
            container.RegisterType<IBoardMonitor, LocalGameBoardMonitor>(new ContainerControlledLifetimeManager());
            container.RegisterType<IBoardOperator, LocalGameBoardOperator>(new ContainerControlledLifetimeManager());
        }
    }
}
