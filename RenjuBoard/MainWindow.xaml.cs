using System.Windows;

namespace RenjuBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void OnNewGameButtonClick(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            if (_mainViewModel != null)
            {
                _mainViewModel.Dispose();
            }
            _mainViewModel = new MainWindowViewModel();
            DataContext = _mainViewModel;
        }
    }
}
