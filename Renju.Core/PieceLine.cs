using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Renju.Core
{
    [DebuggerDisplay("{StartPosition}->{EndPosition}")]
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
        }

        public BoardPosition StartPosition { get; private set; }

        public BoardPosition EndPosition { get; private set; }

        public int Length
        {
            get { return Math.Max(Math.Abs(StartPosition.X - EndPosition.X), Math.Abs(StartPosition.Y - EndPosition.Y)) + 1; }
        }

        public IEnumerable<BoardPoint> Points
        {
            get
            {
                int startX = StartPosition.X;
                int startY = StartPosition.Y;
                int endX = EndPosition.X;
                int endY = EndPosition.Y;

                int offsetX = startX == endX ? 0 : (startX < endX ? 1 : -1);
                int offsetY = startY == endY ? 0 : (startY < endY ? 1 : -1);

                for (int x = startX, y = startY; x <= endX && y <= endY; x += offsetX, y += offsetY)
                {
                    yield return _board[x, y];
                }
            }
        }
    }
}
