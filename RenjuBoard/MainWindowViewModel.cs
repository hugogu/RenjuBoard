using System.Collections.Generic;
using Renju.AI;
using Renju.Core;
using Renju.Core.Rules;
using Runju.Infrastructure;

namespace RenjuBoard
{
    public class MainWindowViewModel : ModelBase
    {
        private GameBoard _gameBoard;
        private AIGamePlayer _aiPlayer = new AIGamePlayer();

        public MainWindowViewModel()
        {
            _gameBoard = new GameBoard(15, new DefaultGameRuleEngine(new IGameRule[]
            {
                new FiveWinRule()
            }));
            _aiPlayer.Side = Side.White;
            _aiPlayer.Board = _gameBoard;
        }

        public GameBoard Board
        {
            get { return _gameBoard; }
        }

        public void DropPoint(BoardPoint point)
        {
            _gameBoard.Drop(point);
        }

        public IEnumerable<BoardPoint> Points
        {
            get { return _gameBoard.Points; }
        }

        public int BoardSize
        {
            get { return _gameBoard.Size; }
        }
    }
}
