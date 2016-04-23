using System.Linq;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Renju.Infrastructure.Model;

namespace Renju.Core
{
    public class CoreModule : IModule
    {
        [Dependency]
        public IUnityContainer Container { get; internal set; }

        public void Initialize()
        {
            Container.RegisterType<IGameBoard<IReadOnlyBoardPoint>, GameBoard>(new PerResolveLifetimeManager());
            Container.RegisterType<IGameRuleEngine, DefaultGameRuleEngine>();
            Container.RegisterTypes(AllClasses.FromLoadedAssemblies().Where(t => typeof(IGameRule).IsAssignableFrom(t)),
                                    WithMappings.FromAllInterfaces,
                                    WithName.TypeName,
                                    WithLifetime.PerResolve);
        }
    }
}
