using System;
using System.Collections.Generic;
using System.Linq;
using Runju.Infrastructure;

namespace Renju.Core
{
    public class VirtualGameBoard<TPoint> : ModelBase, IReadBoardState<TPoint>
        where TPoint : IReadOnlyBoardPoint
    {
        protected readonly List<TPoint> _points;

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

        public IGameRuleEngine RuleEngine { get; protected set; }

        public int Size { get; protected set; }

        public virtual event EventHandler<PieceDropEventArgs> PieceDropped;

        public bool IsDropped(BoardPosition position)
        {
            return this[position].Status.HasValue;
        }

        protected virtual void RaisePeiceDroppedEvent(PieceDrop drop, OperatorType operatorType)
        {
            var temp = PieceDropped;
            if (temp != null)
            {
                temp(this, new PieceDropEventArgs(drop, operatorType));
            }
        }

        protected virtual TPoint GetPoint(BoardPosition position)
        {
            if (position == null)
                throw new ArgumentNullException("position");

            return _points[position.Y * Size + position.X];
        }
    }
}
