using System.Diagnostics;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Renju.Infrastructure.Events;

namespace RenjuBoard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new RenjuBoardBootstrapper().Run();
            Debug.Assert(ServiceLocator.IsLocationProviderSet);
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<StartNewGameEvent>().Publish(NewGameOptions.Default);
        }
    }
}
