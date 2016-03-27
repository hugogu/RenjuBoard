using System;
using System.Collections.Generic;
using System.Linq;

namespace Renju.Core
{
    public class GameBoardDecoration : IGameBoard
    {
        private IGameBoard _decoratedBoard;
        private IReadOnlyBoardPoint _decorationPoint;

        public GameBoardDecoration(IGameBoard board, IReadOnlyBoardPoint decorationPoint)
        {
            if (board[decorationPoint.Position].Status.HasValue)
                throw new InvalidOperationException("Can't decorate with a point already in use.");

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

        public IEnumerable<PieceDrop> Drops
        {
            get { throw new NotSupportedException(); }
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

        public void SetIndex(BoardPosition position, int index)
        {
            throw new NotSupportedException();
        }

        public void SetState(BoardPosition position, Side side)
        {
            throw new NotSupportedException();
        }
    }
}
