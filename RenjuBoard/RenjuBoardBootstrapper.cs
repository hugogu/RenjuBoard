using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            foreach(var type in AllClasses.FromAssembliesInBasePath().Where(t => typeof(IModule).IsAssignableFrom(t)))
            {
                Trace.WriteLine(String.Format("Find module " + type.FullName));
                ModuleCatalog.AddModule(new ModuleInfo(type.Name, type.AssemblyQualifiedName));
            }
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterType(typeof(IEnumerable<>),
                new InjectionFactory((container, type, name) => container.ResolveAll(type.GetGenericArguments().Single())));
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
