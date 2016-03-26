using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Renju.Core
{
    [DebuggerDisplay("({StartPosition.X},{StartPosition.Y})->({EndPosition.X},{EndPosition.Y})")]
    public class PieceLine
    {
        private GameBoard _board;

        public PieceLine(GameBoard board, BoardPosition start, BoardPosition end)
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
        }

        public BoardPosition StartPosition { get; private set; }

        public BoardPosition EndPosition { get; private set; }

        public BoardPosition Direction { get; private set; }

        public int Length
        {
            get { return Math.Max(Math.Abs(StartPosition.X - EndPosition.X), Math.Abs(StartPosition.Y - EndPosition.Y)) + 1; }
        }

        public IEnumerable<BoardPoint> Points
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

        public PieceLine TrimEnd()
        {
            var endPosition = EndPosition;
            while (!_board[endPosition].Status.HasValue)
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
    }
}
