using System.Windows;

namespace RenjuBoard
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new RenjuBoardBootstrapper().Run();
        }
    }
}
