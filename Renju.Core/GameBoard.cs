namespace Renju.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Infrastructure;
    using Infrastructure.Events;
    using Infrastructure.Model;
    using Infrastructure.Model.Extensions;

    [Serializable]
    public class GameBoard : VirtualGameBoard<BoardPoint>, IGameBoard<BoardPoint>, IDisposable
    {
        [NonSerialized]
        private readonly IDisposable _optionsObserver;
        private readonly List<BoardPoint> _droppedPoints = new List<BoardPoint>();
        private Side? _expectedNextTurn = Side.Black;

        public GameBoard(NewGameOptions newGameOptions, GameOptions options)
            : base(newGameOptions.BoardSize, BoardPoint.CreateIndexBasedFactory(newGameOptions.BoardSize))
        {
            Options = options;
            RuleEngine = newGameOptions.RuleEngine;
            _optionsObserver = options.ObserveProperty(() => options.ShowLinesOnBoard)
                                      .Subscribe(_ => OnPropertyChanged(() => Lines));
        }

        public GameOptions Options { get; private set; }

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

        public void Dispose()
        {
            _optionsObserver.Dispose();
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
