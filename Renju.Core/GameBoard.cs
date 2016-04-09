using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Renju.Core
{
    public class GameBoard : VirtualGameBoard<BoardPoint>, IGameBoard<BoardPoint>
    {
        private readonly List<BoardPoint> _droppedPoints = new List<BoardPoint>();
        private Side? _expectedNextTurn = Side.Black;

        public GameBoard(int size, IGameRuleEngine gameRuleEngine)
            : base(size, i => CreateBoardPoint(PositionOfIndex(i, size)))
        {
            RuleEngine = gameRuleEngine;
        }

        public override int DropsCount
        {
            get { return _droppedPoints.Count; }
        }

        public override IEnumerable<BoardPoint> DroppedPoints
        {
            get { return _droppedPoints; }
        }

        public Side? ExpectedNextTurn
        {
            get { return _expectedNextTurn; }
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
            var result = RuleEngine.ProcessDrop(this, drop);
            if (result != DropResult.InvalidDrop)
            {
                var point = GetPoint(drop);
                _expectedNextTurn = result.ExpectedNextSide;
                _droppedPoints.Add(point);
                this.InvalidateNearbyPointsOf(point);
                RaisePeiceDroppedEvent(drop, type);
                RaisePropertyChanged(() => DropsCount);
            }

            return result;
        }

        private static BoardPoint CreateBoardPoint(BoardPosition position)
        {
            return new BoardPoint(position);
        }

        private static BoardPosition PositionOfIndex(int index, int size)
        {
            return new BoardPosition(index % size, index / size);
        }
    }
}
