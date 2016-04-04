using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Runju.Infrastructure;

namespace Renju.Core
{
    public class GameBoard : ModelBase, IGameBoard, IAIPlayground
    {
        private readonly List<BoardPoint> _points;
        private readonly List<BoardPoint> _droppedPoints = new List<BoardPoint>();
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

        public int DropsCount
        {
            get { return _droppedPoints.Count; }
        }

        public IGameRuleEngine RuleEngine
        {
            get { return _gameRuleEngine; }
        }

        public IEnumerable<IReadOnlyBoardPoint> Points
        {
            get { return _points; }
        }

        public IEnumerable<IReadOnlyBoardPoint> DroppedPoints
        {
            get { return _droppedPoints; }
        }

        public Side? ExpectedNextTurn
        {
            get { return _expectedNextTurn; }
        }

        public IReadOnlyBoardPoint this[BoardPosition position]
        {
            get { return GetPoint(position); }
        }

        public bool IsDropped(BoardPosition position)
        {
            return this[position].Status.HasValue;
        }

        public void SetState(BoardPosition position, Side side)
        {
            GetPoint(position).Status = side;
        }

        public void SetIndex(BoardPosition position, int index)
        {
            GetPoint(position).Index = index;
        }

        public DropResult Drop(BoardPosition position, OperatorType type)
        {
            if (_expectedNextTurn.HasValue)
                return Put(new PieceDrop(position, _expectedNextTurn.Value), type);
            else
                return DropResult.InvalidDrop;
        }

        public void Take(BoardPosition position)
        {
            var point = GetPoint(position);
            if (point.Status == null)
                throw new InvalidOperationException(String.Format("{0} hasn't been dropped.", position));
            Contract.Assert(_droppedPoints.Count > 0);
            var lastPoint = _droppedPoints[_droppedPoints.Count - 1];
            if (Equals(point, lastPoint))
            {
                _expectedNextTurn = point.Status.Value;
                point.ResetToEmpty();
                _droppedPoints.RemoveAt(_droppedPoints.Count - 1);
                RaisePropertyChanged(() => DropsCount);
            }
            else
                throw new InvalidOperationException(String.Format("{0} wasn't the last drop.", position));
        }

        protected virtual DropResult Put(PieceDrop drop, OperatorType type)
        {
            var result = _gameRuleEngine.ProcessDrop(this, drop);
            if (result != DropResult.InvalidDrop)
            {
                _expectedNextTurn = result.ExpectedNextSide;
                _droppedPoints.Add(GetPoint(drop));
                RaisePeiceDroppedEvent(drop, type);
                RaisePropertyChanged(() => DropsCount);
            }

            return result;
        }

        protected virtual BoardPoint CreateBoardPoint(BoardPosition position)
        {
            return new BoardPoint(position);
        }

        protected virtual void RaisePeiceDroppedEvent(PieceDrop drop, OperatorType operatorType)
        {
            var temp = PieceDropped;
            if (temp != null)
            {
                temp(this, new PieceDropEventArgs(drop, operatorType));
            }
        }

        private BoardPoint GetPoint(BoardPosition position)
        {
            return _points[position.Y * Size + position.X];
        }

        private BoardPosition PositionOfIndex(int index)
        {
            return new BoardPosition(index % Size, index / Size);
        }
    }
}
