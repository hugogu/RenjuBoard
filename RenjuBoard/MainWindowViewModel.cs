using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Renju.AI;
using Renju.AI.Resolving;
using Renju.AI.Weights;
using Renju.Core;
using Renju.Core.Rules;
using Runju.Infrastructure;

namespace RenjuBoard
{
    public class MainWindowViewModel : ModelBase
    {
        private readonly GameBoard _gameBoard;
        private readonly ICommand _dropPointCommand;
        private readonly DelegateCommand _undoDropCommand;
        private readonly DelegateCommand _redoDropCommand;
        private readonly AIGamePlayer _aiPlayer = new AIGamePlayer(new WinRateGameResolver(new WeightedDropSelector()));
        private readonly BoardRecorder _boardRecorder;

        public MainWindowViewModel()
        {
            _gameBoard = new GameBoard(15, new DefaultGameRuleEngine(new IGameRule[]
            {
                new FiveWinRule()
            }));
            _boardRecorder = new BoardRecorder(_gameBoard);
            _aiPlayer.Side = Side.White;
            _aiPlayer.Board = _gameBoard;
            _dropPointCommand = new DelegateCommand<IReadOnlyBoardPoint>(point => _gameBoard.Drop(point.Position));
            _undoDropCommand = new DelegateCommand(() => _boardRecorder.UndoDrop(), () => _boardRecorder.CanUndo);
            _redoDropCommand = new DelegateCommand(() => _boardRecorder.RedoDrop(), () => _boardRecorder.CanRedo);
            _boardRecorder.PropertyChanged += OnBoardRecorderPropertyChanged;
        }

        private void OnBoardRecorderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _undoDropCommand.RaiseCanExecuteChanged();
                _redoDropCommand.RaiseCanExecuteChanged();
            }));
        }

        public GameBoard Board
        {
            get { return _gameBoard; }
        }

        public ICommand DropPointCommand
        {
            get { return _dropPointCommand; }
        }

        public ICommand UndoCommand
        {
            get { return _undoDropCommand; }
        }

        public ICommand RedoCommand
        {
            get { return _redoDropCommand; }
        }

        public IEnumerable<IReadOnlyBoardPoint> Points
        {
            get { return _gameBoard.Points; }
        }

        public int BoardSize
        {
            get { return _gameBoard.Size; }
        }
    }
}
