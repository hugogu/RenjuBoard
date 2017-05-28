namespace Renju.Infrastructure.Model.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    public static class BoardPositionExtensions
    {
        [Pure]
        public static bool IsOnLineWith(this BoardPosition position, BoardPosition anotherPosition)
        {
            var diff = position - anotherPosition;

            return diff.X == 0 || diff.Y == 0 || Math.Abs(diff.X) == Math.Abs(diff.Y);
        }

        public static int LineDistanceTo(this BoardPosition x, BoardPosition that)
        {
            return Math.Abs(x.X - that.X) + Math.Abs(x.Y - that.Y);
        }

        public static IEnumerable<BoardPosition> StepTo(this BoardPosition from, BoardPosition to, BoardPosition direction)
        {
            Debug.Assert(to != null);
            Debug.Assert(from != null);
            Debug.Assert(from.IsOnLineWith(to));

            var position = from;
            while (!position.Equals(to))
            {
                yield return position;
                position += direction;
            }
            yield return position;
        }

        [Pure]
        public static bool IsOnBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < board.Size && position.Y < board.Size;
        }

        [Pure]
        public static bool IsDropped(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return board[position].Status.HasValue;
        }

        [Pure]
        public static bool IsEmptyAndWithinBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return position.IsOnBoard(board) && !position.IsDropped(board);
        }

        [Pure]
        public static bool IsDroppedBySideOrOutOfBoard(this BoardPosition position, Side side, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return !position.IsOnBoard(board) || (position.IsDropped(board) && board[position].Status == side);
        }

        [Pure]
        public static bool IsInLine(this BoardPosition position, PieceLine line)
        {
            return position.X.InRang(line.StartPosition.X, line.EndPosition.X) &&
                   position.Y.InRang(line.StartPosition.Y, line.EndPosition.Y) &&
                   IsOnLine(position, line);
        }

        [Pure]
        public static bool IsWithInLine(this BoardPosition position, PieceLine line)
        {
            return (position.X.WithInRang(line.StartPosition.X, line.EndPosition.X) ||
                    position.Y.WithInRang(line.StartPosition.Y, line.EndPosition.Y)) &&
                   IsOnLine(position, line);
        }

        [Pure]
        public static bool IsOnLine(this BoardPosition position, PieceLine line)
        {
            return (position.X - line.StartPosition.X) * (position.Y - line.EndPosition.Y) ==
                   (position.X - line.EndPosition.X) * (position.Y - line.StartPosition.Y);
        }
    }
}
