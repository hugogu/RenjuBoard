using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Renju.Infrastructure;
using Renju.Infrastructure.Events;
using Renju.Infrastructure.Model;

namespace Renju.Core
{
    public class GameBoard : VirtualGameBoard<BoardPoint>, IGameBoard<BoardPoint>
    {
        private readonly List<BoardPoint> _droppedPoints = new List<BoardPoint>();
        private Side? _expectedNextTurn = Side.Black;

        public GameBoard(NewGameOptions options)
            : base(options.BoardSize,  BoardPoint.CreateIndexBasedFactory(options.BoardSize))
        {
            Debug.Assert(options.RuleEngine != null);
            RuleEngine = options.RuleEngine;
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

        protected internal override void UpdateLines(IEnumerable<PieceLine> lines)
        {
            RunInDispatcher(() => base.UpdateLines(lines));
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
                UpdateLines(this.FindAllLinesOnBoardWithoutPoint(point).ToList());
                OnPropertyChanged(() => DropsCount);
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
                UpdateLines(this.FindAllLinesOnBoardWithNewPoint(point).ToList());
                RaisePeiceDroppedEvent(drop, type);
                OnPropertyChanged(() => DropsCount);
            }

            return result;
        }
    }
}
