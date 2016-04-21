using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;
using Prism.Unity;
using Renju.Infrastructure.Events;

namespace RenjuBoard
{
    public class RenjuBoardBootstrapper : UnityBootstrapper
    {
        protected override void ConfigureModuleCatalog()
        {
            var mainModuleType = typeof(ApplicationModule);
            ModuleCatalog.AddModule(new ModuleInfo(mainModuleType.Name, mainModuleType.AssemblyQualifiedName));
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = Shell as MainWindow;
            Application.Current.MainWindow.Show();
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            Container.Resolve<IEventAggregator>()
                     .GetEvent<StartNewGameEvent>()
                     .Publish(new NewGameOptions());
        }
    }
}
