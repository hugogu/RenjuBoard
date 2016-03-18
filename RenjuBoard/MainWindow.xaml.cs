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
        private MainWindowViewModel _maiViewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _maiViewModel;
        }

        private void OnBoardPieceMouseUp(object sender, MouseButtonEventArgs e)
        {
            var boardPoint = (sender as FrameworkElement).DataContext as BoardPoint;

            _maiViewModel.DropPoint(boardPoint);
        }
    }
}
