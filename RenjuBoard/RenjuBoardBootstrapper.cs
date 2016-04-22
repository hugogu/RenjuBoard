using System.Diagnostics;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;

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
            Debug.Assert(Application.Current.MainWindow != null);
            Application.Current.MainWindow.Show();
        }
    }
}
