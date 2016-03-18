using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Renju.Core
{
    public class GameBoard
    {
        private List<BoardPoint> _points;
        private IGameRuleEngine _gameRuleEngine;
        private Side? _expectedNextTurn = Side.Black;

        public GameBoard(int size, IGameRuleEngine gameRuleEngine)
        {
            Size = size;
            _gameRuleEngine = gameRuleEngine;
            _points = new List<BoardPoint>(Enumerable.Range(0, size * size).Select(i => CreateBoardPoint()));
        }

        public int Size { get; private set; }

        public IEnumerable<BoardPoint> Points
        {
            get { return new ReadOnlyCollection<BoardPoint>(_points); }
        }

        public BoardPoint this[int x, int y]
        {
            get { return _points[y * Size + x]; }
        }

        public DropResult Drop(BoardPoint point)
        {
            if (_expectedNextTurn.HasValue)
                return ActOnPoint(point, (x, y) => Put(new PieceDrop(x, y, _expectedNextTurn.Value)));
            else
                throw new InvalidOperationException();
        }

        public T ActOnPoint<T>(BoardPoint point, Func<int, int, T> actionOnPoint)
        {
            var index = _points.IndexOf(point);

            return actionOnPoint(index % Size, index / Size);
        }

        public DropResult Put(PieceDrop drop)
        {
            var result = _gameRuleEngine.ProcessDrop(this, drop);
            _expectedNextTurn = result.ExpectedNextSide;

            return result;
        }

        protected virtual BoardPoint CreateBoardPoint()
        {
            return new BoardPoint();
        }
    }
}
