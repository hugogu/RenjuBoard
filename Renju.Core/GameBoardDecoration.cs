namespace Renju.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Debugging;
    using Infrastructure;
    using Infrastructure.Model;
    using Infrastructure.Model.Extensions;

    [Serializable]
    [DebuggerVisualizer(typeof(RenjuBoardVisualizer))]
    public class GameBoardDecoration : ModelBase, IReadBoardState<IReadOnlyBoardPoint>
    {
        private readonly IReadBoardState<IReadOnlyBoardPoint> _decoratedBoard;
        private readonly IReadOnlyBoardPoint _decorationPoint;
        private readonly IEnumerable<PieceLine> _lines;

        public GameBoardDecoration(IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint decorationPoint)
        {
            if (decorationPoint.Position.IsDropped(board))
                throw new InvalidOperationException("Can't decorate with a point already in use.");

            if (!decorationPoint.Status.HasValue)
                throw new ArgumentException("decoration Point much has a Side. ", "decorationPoint");

            var lastPoint = board.Points.Where(p => p.Index.HasValue).OrderByDescending(p => p.Index.Value).FirstOrDefault();
            if ((lastPoint == null && decorationPoint.Status == Side.White) ||
                (lastPoint != null && lastPoint.Status.Value == decorationPoint.Status.Value))
                throw new ArgumentException("Side of decorationPoint is wrong.", "decorationPoint");

            _decoratedBoard = board;
            _decorationPoint = decorationPoint;
            _decoratedBoard.PieceDropped += OnDecoratedBoardPieceDropped;
            _lines = this.FindAllLinesOnBoardWithNewPoint(_decorationPoint).ToList();
            this.InvalidateNearbyPointsOf(decorationPoint);
        }

        [field: NonSerialized]
        public event EventHandler<PieceDropEventArgs> PieceDropped;

        public int DropsCount
        {
            get { return _decoratedBoard.DropsCount + 1; }
        }

        public IEnumerable<IReadOnlyBoardPoint> Points
        {
            get { return _decoratedBoard.Points.Select(p => this[p.Position]); }
        }

        public IEnumerable<IReadOnlyBoardPoint> DroppedPoints
        {
            get { return _decoratedBoard.DroppedPoints.Concat(new[] { _decorationPoint }); }
        }

        public IGameRuleEngine RuleEngine
        {
            get { return _decoratedBoard.RuleEngine; }
        }

        public int Size
        {
            get { return _decoratedBoard.Size; }
        }

        public IEnumerable<PieceLine> Lines
        {
            get { return _lines ?? _decoratedBoard.Lines; }
        }

        public string VisualBoard
        {
            get { return this.GetLiternalPresentation(); }
        }

        public IReadOnlyBoardPoint this[BoardPosition position]
        {
            get { return Equals(_decorationPoint.Position, position) ? _decorationPoint : _decoratedBoard[position]; }
        }

        public IReadOnlyBoardPoint this[int x, int y]
        {
            get { return this[new BoardPosition(x, y)]; }
        }

        protected override void OnConstructingNewObject()
        {
            /* Noop */
        }

        private void OnDecoratedBoardPieceDropped(object sender, PieceDropEventArgs e)
        {
            RaiseEvent(PieceDropped, e);
        }
    }
}
