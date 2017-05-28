namespace Renju.Infrastructure.Model
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Extensions;

    [Serializable]
    [DebuggerVisualizer("Renju.Core.Debugging.RenjuBoardVisualizer, Renju.Core")]
    [DebuggerDisplay("({StartPosition.X},{StartPosition.Y})->({EndPosition.X},{EndPosition.Y})")]
    public class PieceLine : LineSegment
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
            : base(board, start, end, direction)
        {
            Debug.Assert(!Equals(start, end), "Point start and end are the same. ");
            Debug.Assert(start.IsOnLineWith(end), "Point start and end in not on the same line. ");
            Side = Points.Select(p => p.Status).First(s => s.HasValue).Value;

            if (validate)
                ValidatePoint();
        }

        public Side Side { get; private set; }

        public int Weight
        {
            get { return this.GetWeightOnBoard(Board); }
        }

        public int DroppedCount
        {
            get { return Points.Count(p => p.Status.HasValue); }
        }

        public static PieceLine operator +(PieceLine a, PieceLine b)
        {
            Debug.Assert(a != null);
            Debug.Assert(b != null);
            Debug.Assert(a.Board == b.Board, "Two line is not on the same game board.");

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

        [Conditional("DEBUG")]
        private void ValidatePoint()
        {
            Debug.Assert(Points.GroupBy(p => p.Status).Count() <= 2, "A PieceLine shouldn't contains 3 kinds of states.");
        }
    }
}
