namespace RenjuBoard
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;
    using Prism.Regions;
    using Prism.Unity;
    using Renju.Controls;
    using Renju.Core;
    using ViewModels;

    public class RenjuBoardBootstrapper : UnityBootstrapper
    {
        protected override void ConfigureModuleCatalog()
        {
            foreach (var type in AllClasses.FromAssembliesInBasePath().Where(t => typeof(IModule).IsAssignableFrom(t)))
            {
                Trace.WriteLine(String.Format("Find module " + type.FullName));
                ModuleCatalog.AddModule(new ModuleInfo(type.Name, type.AssemblyQualifiedName));
            }
        }

        protected override DependencyObject CreateShell()
        {
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.Logging, typeof(GenericView<LogsViewModel>));

            return new Window()
            {
                Height = 500, Width = 960,
                Title = Application.Current.FindResource("GameWindowTitle") as string
            };
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            App.Current.MainWindow.Show();
            Container.Resolve<GameSessionController<MainWindowViewModel>>().StartNewGame();
        }
    }
}
