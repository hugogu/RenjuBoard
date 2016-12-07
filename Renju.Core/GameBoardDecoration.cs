namespace Renju.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Debugging;
    using Infrastructure;
    using Infrastructure.Events;
    using Infrastructure.Model;
    using Infrastructure.Model.Extensions;

    [Serializable]
    [DebuggerVisualizer(typeof(RenjuBoardVisualizer))]
    public class GameBoardDecoration : DisposableModelBase, IReadBoardState<IReadOnlyBoardPoint>
    {
        private readonly IReadBoardState<IReadOnlyBoardPoint> _decoratedBoard;
        private readonly IReadOnlyBoardPoint _decorationPoint;
        private readonly IEnumerable<PieceLine> _lines;

        public GameBoardDecoration(IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint decorationPoint)
        {
            Contract.Requires(!decorationPoint.Position.IsDropped(board), "Can't decorate with a point already in use.");
            Contract.Requires<ArgumentException>(decorationPoint.Status.HasValue, nameof(decorationPoint) + "much has a Side. ");

            var lastPoint = board.DroppedPoints.LastOrDefault();
            if ((lastPoint == null && decorationPoint.Status == Side.White) ||
                (lastPoint != null && lastPoint.Status.Value == decorationPoint.Status.Value))
                throw new ArgumentException("Side of decorationPoint is wrong.", nameof(decorationPoint));

            _decoratedBoard = board;
            _decorationPoint = decorationPoint;
            _decoratedBoard.Begin += OnDecoratedBoardBegin;
            _decoratedBoard.PieceDropped += OnDecoratedBoardPieceDropped;
            _decoratedBoard.Taken += OnDecoratedBoardPieceTaken;
            _lines = this.FindAllLinesOnBoardWithNewPoint(_decorationPoint).ToList();
            this.InvalidateNearbyPointsOf(decorationPoint);

            AutoCallOnDisposing(() =>
            {
                _decoratedBoard.Begin -= OnDecoratedBoardBegin;
                _decoratedBoard.PieceDropped -= OnDecoratedBoardPieceDropped;
                _decoratedBoard.Taken -= OnDecoratedBoardPieceTaken;
            });
        }

        [field: NonSerialized]
        public event EventHandler<GenericEventArgs<NewGameOptions>> Begin;

        [field: NonSerialized]
        public event EventHandler<PieceDropEventArgs> PieceDropped;

        [field: NonSerialized]
        public event EventHandler<BoardPosition> Taken;

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

        private void OnDecoratedBoardBegin(object sender, GenericEventArgs<NewGameOptions> e)
        {
            RaiseEvent(Begin, e);
        }

        private void OnDecoratedBoardPieceDropped(object sender, PieceDropEventArgs e)
        {
            RaiseEvent(PieceDropped, e);
        }

        private void OnDecoratedBoardPieceTaken(object sender, BoardPosition e)
        {
            RaiseEvent(Taken, e);
        }
    }
}
