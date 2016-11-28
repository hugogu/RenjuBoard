namespace Renju.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Debugging;
    using Infrastructure;
    using Infrastructure.Events;
    using Infrastructure.Model;
    using Infrastructure.Model.Extensions;
    using Microsoft.Practices.Unity.Utility;

    [Serializable]
    [DebuggerVisualizer(typeof(RenjuBoardVisualizer))]
    public class VirtualGameBoard<TPoint> : ModelBase, IReadBoardState<TPoint>
        where TPoint : IReadOnlyBoardPoint
    {
        private readonly ObservableCollection<PieceLine> _lines = new ObservableCollection<PieceLine>();
        private readonly LinkedList<TPoint> _droppedPoints = new LinkedList<TPoint>();
        private readonly List<TPoint> _points;

        public VirtualGameBoard(int size, Func<int, TPoint> createPoint)
        {
            Guard.ArgumentNotNull(createPoint, nameof(createPoint));
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), "size must be postive.");

            Size = size;
            _points = new List<TPoint>(Enumerable.Range(0, size * size).Select(createPoint));
        }

        [field: NonSerialized]
        public event EventHandler<GenericEventArgs<NewGameOptions>> Begin;

        [field: NonSerialized]
        public virtual event EventHandler<PieceDropEventArgs> PieceDropped;

        [field: NonSerialized]
        public virtual event EventHandler<BoardPosition> Taken;

        public virtual IEnumerable<TPoint> DroppedPoints
        {
            get { return _droppedPoints; }
        }

        public virtual int DropsCount
        {
            get { return _droppedPoints.Count; }
        }

        public virtual IEnumerable<TPoint> Points
        {
            get { return _points; }
        }

        public virtual IEnumerable<PieceLine> Lines
        {
            get { return _lines; }
        }

        public string VisualBoard
        {
            get { return this.GetLiternalPresentation(); }
        }

        public IGameRuleEngine RuleEngine { get; protected set; }

        public int Size { get; protected set; }

        public TPoint this[BoardPosition position]
        {
            get { return GetPoint(position); }
        }

        protected internal virtual void UpdateLines(IEnumerable<PieceLine> lines)
        {
            _lines.Clear();
            _lines.AddRange(lines);
        }

        protected virtual void RaiseGameBeginEvent(NewGameOptions options)
        {
            RaiseEvent(Begin, new GenericEventArgs<NewGameOptions>(options));
        }

        protected virtual void RaisePieceTakenEvent(BoardPosition position)
        {
            var point = GetPoint(position);
            _droppedPoints.RemoveLast();
            UpdateLines(((IReadBoardState<IReadOnlyBoardPoint>)this).FindAllLinesOnBoardWithoutPoint(point).ToList());
            RaiseEvent(Taken, position);
            OnPropertyChanged(() => DropsCount);
        }

        protected virtual void RaisePeiceDroppedEvent(PieceDrop drop, OperatorType operatorType)
        {
            var point = GetPoint(drop);
            _droppedPoints.AddLast(point);
            ((IReadBoardState<IReadOnlyBoardPoint>)this).InvalidateNearbyPointsOf(point);
            UpdateLines(((IReadBoardState<IReadOnlyBoardPoint>)this).FindAllLinesOnBoardWithNewPoint(point).ToList());
            RaiseEvent(PieceDropped, new PieceDropEventArgs(drop, operatorType));
            OnPropertyChanged(() => DropsCount);
        }

        protected virtual TPoint GetPoint(BoardPosition position)
        {
            Guard.ArgumentNotNull(position, nameof(position));

            return _points[position.Y * Size + position.X];
        }
    }
}
