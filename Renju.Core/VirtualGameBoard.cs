using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Renju.Infrastructure;

namespace Renju.Core
{
    public class VirtualGameBoard<TPoint> : ModelBase, IReadBoardState<TPoint>
        where TPoint : IReadOnlyBoardPoint
    {
        protected readonly ObservableCollection<PieceLine> _lines = new ObservableCollection<PieceLine>();
        private readonly List<TPoint> _points;

        public VirtualGameBoard(int size, Func<int, TPoint> createPoint)
        {
            if (createPoint == null)
                throw new ArgumentNullException("createPoint");

            if (size <= 0)
                throw new ArgumentOutOfRangeException("size", "size must be postive.");

            Size = size;
            _points = new List<TPoint>(Enumerable.Range(0, size * size).Select(createPoint));
        }

        public TPoint this[BoardPosition position]
        {
            get { return GetPoint(position); }
        }

        public virtual IEnumerable<TPoint> DroppedPoints
        {
            get { throw new NotSupportedException(); }
        }

        public virtual int DropsCount
        {
            get { throw new NotSupportedException(); }
        }

        public virtual IEnumerable<TPoint> Points
        {
            get { return _points; }
        }

        public virtual IEnumerable<PieceLine> Lines
        {
            get { return _lines; }
        }

        public IGameRuleEngine RuleEngine { get; protected set; }

        public int Size { get; protected set; }

        public virtual event EventHandler<PieceDropEventArgs> PieceDropped;

        protected internal virtual void UpdateLines(IEnumerable<PieceLine> lines)
        {
            _lines.Clear();
            _lines.AddRange(lines);
        }

        protected virtual void RaisePeiceDroppedEvent(PieceDrop drop, OperatorType operatorType)
        {
            RaiseEvent(PieceDropped, new PieceDropEventArgs(drop, operatorType));
        }

        protected virtual TPoint GetPoint(BoardPosition position)
        {
            if (position == null)
                throw new ArgumentNullException("position");

            return _points[position.Y * Size + position.X];
        }
    }
}
