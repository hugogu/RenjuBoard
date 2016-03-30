using System.Windows;
using System.Windows.Input;
using Renju.Core;

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

        private void OnBoardPieceMouseUp(object sender, MouseButtonEventArgs e)
        {
            var boardPoint = (sender as FrameworkElement).DataContext as BoardPoint;

            _mainViewModel.DropPoint(boardPoint);
        }

        private void OnNewGameButtonClick(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void OnUndoButtonClick(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Board.UndoLastDrop();
        }

        private void StartNewGame()
        {
            _mainViewModel = new MainWindowViewModel();
            DataContext = _mainViewModel;
        }

        private void OnRedoButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
