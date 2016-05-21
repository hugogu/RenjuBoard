namespace Renju.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Practices.Unity.Utility;
    using Debugging;
    using Infrastructure;
    using Infrastructure.Model;
    using Infrastructure.Model.Extensions;

    [Serializable]
    [DebuggerVisualizer(typeof(RenjuBoardVisualizer))]
    public class VirtualGameBoard<TPoint> : ModelBase, IReadBoardState<TPoint>
        where TPoint : IReadOnlyBoardPoint
    {
        private readonly ObservableCollection<PieceLine> _lines = new ObservableCollection<PieceLine>();
        private readonly List<TPoint> _points;

        public VirtualGameBoard(int size, Func<int, TPoint> createPoint)
        {
            Guard.ArgumentNotNull(createPoint, "createPoint");
            if (size <= 0)
                throw new ArgumentOutOfRangeException("size", "size must be postive.");

            Size = size;
            _points = new List<TPoint>(Enumerable.Range(0, size * size).Select(createPoint));
        }

        [field: NonSerialized]
        public virtual event EventHandler<PieceDropEventArgs> PieceDropped;

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
            this._lines.Clear();
            this._lines.AddRange(lines);
        }

        protected virtual void RaisePeiceDroppedEvent(PieceDrop drop, OperatorType operatorType)
        {
            RaiseEvent(PieceDropped, new PieceDropEventArgs(drop, operatorType));
        }

        protected virtual TPoint GetPoint(BoardPosition position)
        {
            Guard.ArgumentNotNull(position, "position");

            return _points[position.Y * Size + position.X];
        }
    }
}
