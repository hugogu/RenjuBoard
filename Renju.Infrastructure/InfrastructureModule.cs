using Microsoft.Practices.Unity;
using Prism.Modularity;
using Renju.Infrastructure.Execution;

namespace Renju.Infrastructure
{
    public class InfrastructureModule : IModule
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        public void Initialize()
        {
            Container.RegisterType<IStepController, ExecutionStepController>(new PerResolveLifetimeManager());
        }
    }
}
