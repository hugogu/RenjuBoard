namespace RenjuBoard.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Practices.Unity;
    using Prism.Commands;
    using Renju.Core;
    using Renju.Infrastructure;
    using Renju.Infrastructure.Model;
    using Renju.Infrastructure.Protocols;

    public class MainWindowViewModel : DisposableModelBase
    {
        public MainWindowViewModel(GameOptions options)
        {
            DropPointCommand = new DelegateCommand<IReadOnlyBoardPoint>(OnDroppingPiece);
            NewGameCommand = new DelegateCommand(() => GameSessionController.StartNewGame());
            AutoDispose(options.ObserveProperty(() => options.ShowLinesOnBoard).Subscribe(_ => OnPropertyChanged(() => Lines)));
        }

        [Dependency]
        public GameSessionController<MainWindowViewModel> GameSessionController { get; set; }

        [Dependency]
        public BoardRecorder BoardRecorder { get; internal set; }

        [Dependency]
        public AIControllerViewModel AIControllerVM { get; internal set; }

        [Dependency]
        public IBoardMonitor Board { get; internal set; }

        [Dependency]
        public BoardTimingViewModel TimingVM { get; internal set; }

        [Dependency]
        public SaveAndLoadViewModel SaveLoadVM { get; internal set; }

        [Dependency]
        public IGameBoard<IReadOnlyBoardPoint> GameBoard { get; internal set; }

        [Dependency]
        public OptionsViewModel OptionsVM { get; internal set; }

        public IEnumerable<PieceLine> Lines
        {
            get { return OptionsVM.Options.ShowLinesOnBoard ? GameBoard.Lines : new PieceLine[0]; }
        }

        public ICommand DropPointCommand { get; private set; }

        public ICommand NewGameCommand { get; private set; }

        private void OnDroppingPiece(IReadOnlyBoardPoint point)
        {
            try
            {
                GameBoard.Drop(point.Position, OperatorType.Human);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Renju Error", MessageBoxButton.OK);
            }
        }
    }
}
