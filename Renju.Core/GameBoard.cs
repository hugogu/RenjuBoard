using System;
using System.Collections.Generic;
using System.Linq;

namespace Renju.Core
{
    public class GameBoard : IGameBoard, IAIPlayground
    {
        private readonly List<BoardPoint> _points;
        private readonly IGameRuleEngine _gameRuleEngine;
        private Side? _expectedNextTurn = Side.Black;

        public GameBoard(int size, IGameRuleEngine gameRuleEngine)
        {
            Size = size;
            _gameRuleEngine = gameRuleEngine;
            _points = new List<BoardPoint>(Enumerable.Range(0, size * size).Select(i => CreateBoardPoint(PositionOfIndex(i))));
        }

        public virtual event EventHandler<PieceDropEventArgs> PieceDropped;

        public int Size { get; private set; }

        public IGameRuleEngine RuleEngine
        {
            get { return _gameRuleEngine; }
        }

        public IEnumerable<IReadOnlyBoardPoint> Points
        {
            get { return _points; }
        }

        public Side? ExpectedNextTurn
        {
            get { return _expectedNextTurn; }
        }

        public IReadOnlyBoardPoint this[int x, int y]
        {
            get { return GetPoint(x, y); }
        }

        public IReadOnlyBoardPoint this[BoardPosition position]
        {
            get { return GetPoint(position.X, position.Y); }
        }

        public void SetState(BoardPosition position, Side side)
        {
            GetPoint(position.X, position.Y).Status = side;
        }

        public void SetIndex(BoardPosition position, int index)
        {
            GetPoint(position.X, position.Y).Index = index;
        }

        public DropResult Drop(BoardPosition position)
        {
            if (_expectedNextTurn.HasValue)
                return Put(new PieceDrop(position.X, position.Y, _expectedNextTurn.Value));
            else
                return DropResult.InvalidDrop;
        }

        public void Take(BoardPosition position)
        {
            var point = GetPoint(position.X, position.Y);
            if (point.Status == null)
                throw new InvalidOperationException(String.Format("{0} hasn't been dropped.", position));
            _expectedNextTurn = point.Status.Value;
            point.ResetToEmpty();
        }

        protected virtual DropResult Put(PieceDrop drop)
        {
            var result = _gameRuleEngine.ProcessDrop(this, drop);
            if (result != DropResult.InvalidDrop)
            {
                _expectedNextTurn = result.ExpectedNextSide;
                RaisePeiceDroppedEvent(drop);
            }

            return result;
        }

        protected virtual BoardPoint CreateBoardPoint(BoardPosition position)
        {
            return new BoardPoint(position);
        }

        protected virtual void RaisePeiceDroppedEvent(PieceDrop drop)
        {
            var temp = PieceDropped;
            if (temp != null)
            {
                temp(this, new PieceDropEventArgs(drop));
            }
        }

        private BoardPoint GetPoint(int x, int y)
        {
            return _points[y * Size + x];
        }

        private BoardPosition PositionOfIndex(int index)
        {
            return new BoardPosition(index % Size, index / Size);
        }
    }
}
