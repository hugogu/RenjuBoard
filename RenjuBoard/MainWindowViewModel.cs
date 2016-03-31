using System.Collections.Generic;
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
        private GameBoard _gameBoard;
        private ICommand _dropPointCommand;
        private AIGamePlayer _aiPlayer = new AIGamePlayer(new WinRateGameResolver(new WeightedDropSelector()));

        public MainWindowViewModel()
        {
            _gameBoard = new GameBoard(15, new DefaultGameRuleEngine(new IGameRule[]
            {
                new FiveWinRule()
            }));
            _aiPlayer.Side = Side.White;
            _aiPlayer.Board = _gameBoard;
            _dropPointCommand = new DelegateCommand<IReadOnlyBoardPoint>(point => _gameBoard.Drop(point));
        }

        public GameBoard Board
        {
            get { return _gameBoard; }
        }

        public ICommand DropPointCommand
        {
            get { return _dropPointCommand; }
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
