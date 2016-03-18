using System.Collections.Generic;
using Renju.Core;
using Renju.Core.Rules;
using Runju.Infrastructure;

namespace RenjuBoard
{
    public class MainWindowViewModel : ModelBase
    {
        private GameBoard _gameBoard;

        public MainWindowViewModel()
        {
            _gameBoard = new GameBoard(15, new DefaultGameRuleEngine(new IGameRule[]
            {
                new FiveWinRule()
            }));
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
