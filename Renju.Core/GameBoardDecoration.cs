using System;
using System.Collections.Generic;
using System.Linq;

namespace Renju.Core
{
    public class GameBoardDecoration : IReadBoardState
    {
        private IReadBoardState _decoratedBoard;
        private IReadOnlyBoardPoint _decorationPoint;

        public GameBoardDecoration(IReadBoardState board, IReadOnlyBoardPoint decorationPoint)
        {
            if (board[decorationPoint.Position].Status.HasValue)
                throw new InvalidOperationException("Can't decorate with a point already in use.");

            if (!decorationPoint.Status.HasValue)
                throw new ArgumentException("decoration Point much has a Side. ", "decorationPoint");

            var lastPoint = board.Points.Where(p => p.Index.HasValue).OrderByDescending(p => p.Index.Value).FirstOrDefault();
            if ((lastPoint == null && decorationPoint.Status == Side.White) ||
                (lastPoint != null && lastPoint.Status.Value == decorationPoint.Status.Value))
                throw new ArgumentException("Side of decorationPoint is wrong.", "decorationPoint");

            _decoratedBoard = board;
            _decorationPoint = decorationPoint;
        }

        public IReadOnlyBoardPoint this[BoardPosition position]
        {
            get
            {
                var point = _decoratedBoard[position];
                if (point == null && Equals(_decorationPoint.Position, position))
                {
                    point = _decorationPoint;
                }
                return point;
            }
        }

        public IReadOnlyBoardPoint this[int x, int y]
        {
            get { return this[new BoardPosition(x, y)]; }
        }

        public IEnumerable<IReadOnlyBoardPoint> Points
        {
            get { return _decoratedBoard.Points.Concat(new[] { _decorationPoint }); }
        }

        public IGameRuleEngine RuleEngine
        {
            get { return _decoratedBoard.RuleEngine; }
        }

        public int Size
        {
            get { return _decoratedBoard.Size; }
        }
    }
}
