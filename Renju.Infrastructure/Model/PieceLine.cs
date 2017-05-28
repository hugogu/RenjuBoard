namespace Renju.Infrastructure.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using Extensions;

    [Serializable]
    [DebuggerVisualizer("Renju.Core.Debugging.RenjuBoardVisualizer, Renju.Core")]
    [DebuggerDisplay("({StartPosition.X},{StartPosition.Y})->({EndPosition.X},{EndPosition.Y})")]
    public class PieceLine
    {
        public PieceLine(IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition start, BoardPosition end)
            : this(board, start, end, true)
        {
        }

        protected internal PieceLine(IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition start, BoardPosition end, bool validate)
            : this(board, start, end, new BoardPosition(GetDirection(start.X, end.X), GetDirection(start.Y, end.Y)), validate)
        {
        }

        protected internal PieceLine(IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition start, BoardPosition end, BoardPosition direction, bool validate = false)
        {
            Debug.Assert(!Equals(start, end), "Point start and end are the same. ");
            Debug.Assert(start.IsOnLineWith(end), "Point start and end in not on the same line. ");

            Board = board;
            StartPosition = start;
            EndPosition = end;
            Direction = direction;
            Side = Points.Select(p => p.Status).First(s => s.HasValue).Value;

            if (validate)
                ValidatePoint();
        }

        public BoardPosition StartPosition { get; private set; }

        public BoardPosition EndPosition { get; private set; }

        public Point MiddlePosition
        {
            get { return new Point(((double)StartPosition.X + EndPosition.X) / 2, ((double)StartPosition.Y + EndPosition.Y) / 2); }
        }

        public BoardPosition Direction { get; private set; }

        public Side Side { get; private set; }

        public IReadBoardState<IReadOnlyBoardPoint> Board { get; private set; }

        public int Weight
        {
            get { return this.GetWeightOnBoard(Board); }
        }

        public int Length
        {
            get { return Math.Max(Math.Abs(StartPosition.X - EndPosition.X), Math.Abs(StartPosition.Y - EndPosition.Y)) + 1; }
        }

        public IEnumerable<IReadOnlyBoardPoint> DroppedPoints
        {
            get { return Points.Where(p => p.Status != null); }
        }

        public IEnumerable<IReadOnlyBoardPoint> Points
        {
            get { return Positions.Where(p => p.IsOnBoard(Board)).Select(p => Board[p]); }
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

        public int DroppedCount
        {
            get { return Points.Count(p => p.Status.HasValue); }
        }

        public IReadOnlyBoardPoint this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < Length);

                return Board[StartPosition + Direction * index];
            }
        }

        public static PieceLine operator +(PieceLine a, PieceLine b)
        {
            if (a == null || b == null)
                return null;
            Debug.Assert(a?.Board == b?.Board, "Two line is not on the same game board.");

            if (a.Side != b.Side)
                return null;

            if (!Equals(a.Direction, b.Direction) && !Equals(a.Direction, -b.Direction))
                return null;

            if (Equals(a.StartPosition, b.EndPosition))
                return new PieceLine(a.Board, b.StartPosition, a.EndPosition);
            else if (Equals(a.StartPosition, b.StartPosition))
                return new PieceLine(a.Board, a.EndPosition, b.EndPosition);
            else if (Equals(a.EndPosition, b.StartPosition))
                return new PieceLine(a.Board, a.StartPosition, b.EndPosition);
            else if (Equals(a.EndPosition, b.EndPosition))
                return new PieceLine(a.Board, a.StartPosition, b.StartPosition);
            else
                return null;
        }

        public static PieceLine operator +(PieceLine line, int offset)
        {
            return new PieceLine(line.Board, line.StartPosition, line.EndPosition + line.Direction * offset, line.Direction);
        }

        public static PieceLine operator -(PieceLine line, int offset)
        {
            return new PieceLine(line.Board, line.StartPosition, line.EndPosition - line.Direction * offset, line.Direction);
        }

        public static PieceLine operator +(int offset, PieceLine line)
        {
            return new PieceLine(line.Board, line.StartPosition - line.Direction * offset, line.EndPosition, line.Direction);
        }

        public static PieceLine operator -(int offset, PieceLine line)
        {
            return new PieceLine(line.Board, line.StartPosition + line.Direction * offset, line.EndPosition, line.Direction);
        }

        public string GetPatternStringOfSide(BoardPosition drop, Side side)
        {
            Debug.Assert(Board[drop].Status == null);

            return String.Concat(Positions.Select(p => p.Equals(drop) ? "." : (p.IsOnBoard(Board) ? Board[p].GetPatterOnSide(side) : "_")));
        }

        public PieceLine ResizeTo(int length, BoardPosition centerPoint)
        {
            Debug.Assert(centerPoint.IsInLine(this));

            var result = this;
            while (result.Length < length)
            {
                if (centerPoint.LineDistanceTo(result.StartPosition) < centerPoint.LineDistanceTo(result.EndPosition))
                {
                    result = 1 + result;
                }
                else
                {
                    result = result + 1;
                }
            }
            while(result.Length > length)
            {
                if (centerPoint.LineDistanceTo(result.StartPosition) < centerPoint.LineDistanceTo(result.EndPosition)
                    && (result.DroppedPoints.Count() > 1 || Board[EndPosition].Status == null))
                {
                    result = result - 1;
                }
                else
                {
                    result = 1 - result;
                }
            }

            return result;
        }

        public PieceLine Trim()
        {
            var trimEnd = TrimEnd();

            return trimEnd == null ? null : trimEnd.TrimStart();
        }

        public PieceLine TrimStart()
        {
            var startPosition = StartPosition;
            while (!startPosition.IsDropped(Board))
            {
                startPosition += Direction;
                if (Equals(startPosition, EndPosition))
                    return null;
            }

            return new PieceLine(Board, startPosition, EndPosition);
        }

        public PieceLine TrimEnd()
        {
            var endPosition = EndPosition;
            while (!endPosition.IsDropped(Board))
            {
                endPosition -= Direction;
                if (Equals(StartPosition, endPosition))
                    return null;
            }

            return new PieceLine(Board, StartPosition, endPosition);
        }

        internal static int GetDirection(int a, int b)
        {
            return a == b ? 0 : a < b ? 1 : -1;
        }

        [Conditional("DEBUG")]
        private void ValidatePoint()
        {
            Debug.Assert(Points.GroupBy(p => p.Status).Count() <= 2, "A PieceLine shouldn't contains 3 kinds of states.");
        }
    }
}
