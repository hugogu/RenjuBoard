namespace RenjuBoard.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Practices.Unity;
    using Prism.Commands;
    using Prism.Events;
    using Renju.Core;
    using Renju.Infrastructure;
    using Renju.Infrastructure.Events;
    using Renju.Infrastructure.Model;

    public class MainWindowViewModel : DisposableModelBase
    {
        public MainWindowViewModel(IEventAggregator eventAggregator, GameOptions options)
        {
            DropPointCommand = new DelegateCommand<IReadOnlyBoardPoint>(OnDroppingPiece);
            NewGameCommand = new DelegateCommand(() => eventAggregator.GetEvent<StartNewGameEvent>().Publish(NewGameOptions.Default));
            AutoDispose(options.ObserveProperty(() => options.ShowLinesOnBoard).Subscribe(_ => OnPropertyChanged(() => Lines)));
        }

        [Dependency]
        public BoardRecorder BoardRecorder { get; internal set; }

        [Dependency]
        public AIControllerViewModel AIControllerVM { get; internal set; }

        [Dependency]
        public BoardTimingViewModel TimingVM { get; internal set; }

        [Dependency]
        public SaveAndLoadViewModel SaveLoadVM { get; internal set; }

        [Dependency]
        public LogsViewModel LogViewModel { get; internal set; }

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
