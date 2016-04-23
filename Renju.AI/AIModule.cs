using Microsoft.Practices.Unity;
using Prism.Modularity;
using Renju.AI.Resolving;
using Renju.AI.Weights;
using Renju.Infrastructure.Model;

namespace Renju.AI
{
    public class AIModule : IModule
    {
        [Dependency]
        public IUnityContainer Container { get; internal set; }

        public void Initialize()
        {
            Container.RegisterType<IDropSelector, WeightedDropSelector>(new PerResolveLifetimeManager());
            Container.RegisterType<IDropResolver, WinRateGameResolver>(new PerResolveLifetimeManager());
            Container.RegisterType<IGamePlayer, AIGamePlayer>(new PerResolveLifetimeManager());
        }
    }
}
