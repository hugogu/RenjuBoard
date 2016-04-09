using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Renju.Core
{
    [DebuggerDisplay("({StartPosition.X},{StartPosition.Y})->({EndPosition.X},{EndPosition.Y})")]
    public class PieceLine
    {
        private readonly IReadBoardState<IReadOnlyBoardPoint> _board;

        public PieceLine(IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition start, BoardPosition end)
        {
            if (start.X != end.X &&
                start.Y != end.Y &&
                Math.Abs(start.X - end.X) != Math.Abs(start.Y - end.Y))
                throw new ArgumentException(String.Format("Point {0} and {1} in not on the same line. ", start, end));

            if (start.X == end.X && start.Y == end.Y)
                throw new ArgumentException(String.Format("Point {0} and {1} are the same. ", start, end));

            _board = board;
            StartPosition = start;
            EndPosition = end;
            Direction = new BoardPosition(GetDirection(start.X, end.X), GetDirection(start.Y, end.Y));
            ValidatePoint();
            Side = Points.Select(p => p.Status).Where(s => s.HasValue).First().Value;
        }

        public BoardPosition StartPosition { get; private set; }

        public BoardPosition EndPosition { get; private set; }

        public Point MiddlePosition
        {
            get { return new Point(((double)StartPosition.X + EndPosition.X) / 2, ((double)StartPosition.Y + EndPosition.Y) / 2); }
        }

        public BoardPosition Direction { get; private set; }

        public Side Side { get; private set; }

        public IReadBoardState<IReadOnlyBoardPoint> Board { get { return _board; } }

        public int Weight { get { return this.GetWeightOnBoard(_board); } }

        public bool IsClosed { get { return IsEndClosed || IsStartClosed; } }

        public bool IsEndClosed
        {
            get
            {
                var endPos = EndPosition + Direction;
                return !endPos.IsOnBoard(_board) || (_board.IsDropped(endPos) && _board[endPos].Status != _board[EndPosition].Status);
            }
        }

        public bool IsStartClosed
        {
            get
            {
                var beforePos = StartPosition - Direction;
                return !beforePos.IsOnBoard(_board) || (_board.IsDropped(beforePos) && _board[beforePos].Status != _board[StartPosition].Status);
            }
        }

        public int Length
        {
            get { return Math.Max(Math.Abs(StartPosition.X - EndPosition.X), Math.Abs(StartPosition.Y - EndPosition.Y)) + 1; }
        }

        public IEnumerable<IReadOnlyBoardPoint> Points
        {
            get
            {
                var position = StartPosition;
                while (!Equals(position, EndPosition))
                {
                    yield return _board[position];
                    position += Direction;
                }
                yield return _board[EndPosition];
            }
        }

        public int DroppedCount
        {
            get { return Points.Count(p => p.Status.HasValue); }
        }

        public static PieceLine operator +(PieceLine a, PieceLine b)
        {
            if (a == null || b == null)
                return null;

            if (a._board != b._board)
                throw new InvalidOperationException("Two line is not on the same game board.");

            if (a.Side != b.Side)
                return null;

            if (!Equals(a.Direction, b.Direction) && !Equals(a.Direction, b.Direction.GetOpposite()))
                return null;

            if (Equals(a.StartPosition, b.EndPosition))
                return new PieceLine(a._board, b.StartPosition, a.EndPosition);
            else if (Equals(a.StartPosition, b.StartPosition))
                return new PieceLine(a._board, a.EndPosition, b.EndPosition);
            else if (Equals(a.EndPosition, b.StartPosition))
                return new PieceLine(a._board, a.StartPosition, b.EndPosition);
            else if (Equals(a.EndPosition, b.EndPosition))
                return new PieceLine(a._board, a.StartPosition, b.StartPosition);
            else
                return null;
        }

        public PieceLine TrimEnd()
        {
            var endPosition = EndPosition;
            while (!_board.IsDropped(endPosition))
            {
                endPosition -= Direction;
                if (Equals(StartPosition, endPosition))
                    return null;
            }

            return new PieceLine(_board, StartPosition, endPosition);
        }

        internal static int GetDirection(int a, int b)
        {
            return a == b ? 0 : a < b ? 1 : -1;
        }

        [Conditional("DEBUG")]
        private void ValidatePoint()
        {
            if (Points.GroupBy(p => p.Status).Count() > 2)
                throw new InvalidOperationException("A PieceLine shouldn't contains 3 kinds of states.");
        }
    }
}
