namespace Renju.Infrastructure.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using Extensions;

    [Serializable]
    [DebuggerDisplay("({StartPosition.X},{StartPosition.Y})->({EndPosition.X},{EndPosition.Y})")]
    public class LineSegment
    {
        public BoardPosition StartPosition { get; protected set; }

        public BoardPosition EndPosition { get; protected set; }

        public IReadBoardState<IReadOnlyBoardPoint> Board { get; protected set; }

        public BoardPosition Direction { get; protected set; }

        public LineSegment(IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition start, BoardPosition end)
            : this(board, start, end, new BoardPosition(GetDirection(start.X, end.X), GetDirection(start.Y, end.Y)))
        {
        }

        public LineSegment(IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition start, BoardPosition end, BoardPosition direction)
        {
            Board = board;
            EndPosition = end;
            StartPosition = start;
            Direction = direction;
        }

        public Point MiddlePosition
        {
            get { return new Point(((double)StartPosition.X + EndPosition.X) / 2, ((double)StartPosition.Y + EndPosition.Y) / 2); }
        }

        public int Length
        {
            get { return Math.Max(Math.Abs(StartPosition.X - EndPosition.X), Math.Abs(StartPosition.Y - EndPosition.Y)) + 1; }
        }

        public IEnumerable<IReadOnlyBoardPoint> Points
        {
            get { return Positions.Where(p => p.IsOnBoard(Board)).Select(p => Board[p]); }
        }

        public IEnumerable<IReadOnlyBoardPoint> DroppedPoints
        {
            get { return Points.Where(p => p.Status.HasValue); }
        }

        public IEnumerable<BoardPosition> Positions
        {
            get
            {
                var position = StartPosition;
                while (!Equals(position, EndPosition))
                {
                    yield return position;
                    position += Direction;
                }
                yield return position;
            }
        }

        public IReadOnlyBoardPoint this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < Length);

                return Board[StartPosition + Direction * index];
            }
        }

        public string GetPatternStringOfSide(BoardPosition drop, Side side)
        {
            Debug.Assert(Board[drop].Status == null);

            return String.Concat(Positions.Select(p => p.Equals(drop) ? "." : (p.IsOnBoard(Board) ? Board[p].GetPatterOnSide(side) : "_")));
        }

        internal static int GetDirection(int a, int b)
        {
            return a == b ? 0 : a < b ? 1 : -1;
        }
    }
}
