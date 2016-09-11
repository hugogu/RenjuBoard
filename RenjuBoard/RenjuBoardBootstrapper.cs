namespace RenjuBoard
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;
    using Prism.Regions;
    using Prism.Unity;
    using Renju.Controls;
    using Renju.Infrastructure;
    using ViewModels;

    public class RenjuBoardBootstrapper : UnityBootstrapper
    {
        protected override void ConfigureModuleCatalog()
        {
            foreach (var type in typeof(IModule).FindAllImplementations())
            {
                Trace.WriteLine(String.Format("Find module " + type.FullName));
                ModuleCatalog.AddModule(new ModuleInfo(type.Name, type.AssemblyQualifiedName));
            }
        }

        protected override DependencyObject CreateShell()
        {
            Container.Resolve<IRegionManager>()
                     .RegisterViewWithRegion(RegionNames.Logging, typeof(GenericView<LogsViewModel>))
                     .RegisterViewWithRegion(RegionNames.Main, typeof(GenericView<MainWindowViewModel>));

            var shell = new Window()
            {
                Height = 500, Width = 960,
                Title = Application.Current.FindResource("GameWindowTitle") as string
            };

            return shell;
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            App.Current.MainWindow.Show();
            Container.Resolve<GameSessionController>().StartNewGame();
        }
    }
}
