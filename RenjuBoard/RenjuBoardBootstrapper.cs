using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using Renju.Infrastructure.ViewModel;

namespace RenjuBoard
{
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
            return new Window() { Height = 500, Width = 960, Title = Application.Current.FindResource("GameWindowTitle") as string };
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            VMInfo.SetDataType(Shell, typeof(MainWindowViewModel));
            App.Current.MainWindow.Show();
        }
    }
}
