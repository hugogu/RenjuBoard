using System;
using System.Collections.Generic;
using System.Linq;
using Renju.Infrastructure;

namespace Renju.Core
{
    public class GameBoardDecoration : ModelBase, IReadBoardState<IReadOnlyBoardPoint>
    {
        private readonly IReadBoardState<IReadOnlyBoardPoint> _decoratedBoard;
        private readonly IReadOnlyBoardPoint _decorationPoint;

        public GameBoardDecoration(IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint decorationPoint)
        {
            if (board.IsDropped(decorationPoint.Position))
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
            this.InvalidateNearbyPointsOf(decorationPoint);
        }

        public IReadOnlyBoardPoint this[BoardPosition position]
        {
            get { return Equals(_decorationPoint.Position, position) ? _decorationPoint : _decoratedBoard[position]; }
        }

        public IReadOnlyBoardPoint this[int x, int y]
        {
            get { return this[new BoardPosition(x, y)]; }
        }

        public int DropsCount
        {
            get { return _decoratedBoard.DropsCount + 1; }
        }

        public IEnumerable<IReadOnlyBoardPoint> Points
        {
            get
            {
                foreach (var point in _decoratedBoard.Points)
                {
                    if (Equals(point.Position, _decorationPoint.Position))
                        yield return _decorationPoint;
                    else
                        yield return point;
                }
            }
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
            get { return _decoratedBoard.FindAllLinesOnBoardWithNewPoint(_decorationPoint); }
        }

        public event EventHandler<PieceDropEventArgs> PieceDropped;

        public bool IsDropped(BoardPosition position)
        {
            return Equals(_decorationPoint.Position, position) || _decoratedBoard.IsDropped(position);
        }

        private void OnDecoratedBoardPieceDropped(object sender, PieceDropEventArgs e)
        {
            RaiseEvent(PieceDropped, e);
        }
    }
}
