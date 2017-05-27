namespace Renju.Infrastructure
{
    using System;
    using AI;
    using AI.Weight;
    using Execution;
    using IO;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;
    using Protocols;
    using Protocols.Local;

    public class InfrastructureModule : IModule
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        public void Initialize()
        {
            Container.RegisterInstance<Action<IUnityContainer>>(GetType().Name, RegisterTypes);
            Container.RegisterType<IDropWeightEvaluator, WeightingStrategy>(new ContainerControlledLifetimeManager(), new InjectionConstructor("AIWeightingStrategy.txt"));
            Container.RegisterType<IMessenger<string, string>>(
                "Console",
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(c => c.BuildUp(new StreamMessenger<string, string>(Console.OpenStandardInput(), Console.OpenStandardOutput()))));
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IDropSelector, PrioritizedDropsSelector>(new PerResolveLifetimeManager());
            //container.RegisterType<IDropSelector, WeightPatternDropSelector>(new PerResolveLifetimeManager());
            container.RegisterType<IStepController, ExecutionStepController>(new ContainerControlledLifetimeManager());
            container.RegisterType<IBoardMonitor, LocalGameBoardMonitor>(new ContainerControlledLifetimeManager());
            container.RegisterType<IBoardOperator, LocalGameBoardOperator>(new ContainerControlledLifetimeManager());
        }
    }
}
