using System;

namespace Renju.Infrastructure.Model.Extensions
{
    public static class BoardPositionExtensions
    {
        public static bool IsOnLineWith(this BoardPosition position, BoardPosition anotherPosition)
        {
            var diff = position - anotherPosition;

            return diff.X == 0 || diff.Y == 0 || Math.Abs(diff.X) == Math.Abs(diff.Y);
        }

        public static bool IsOnBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < board.Size && position.Y < board.Size;
        }

        public static bool IsDropped(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return board[position].Status.HasValue;
        }

        public static bool IsEmptyAndWithinBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return position.IsOnBoard(board) && !position.IsDropped(board);
        }

        public static bool IsDroppedBySideOrOutOfBoard(this BoardPosition position, Side side, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return !position.IsOnBoard(board) || (position.IsDropped(board) && board[position].Status == side);
        }

        public static bool IsInLine(this BoardPosition position, PieceLine line)
        {
            return position.X.InRang(line.StartPosition.X, line.EndPosition.X) &&
                   position.Y.InRang(line.StartPosition.Y, line.EndPosition.Y) &&
                   IsOnLine(position, line);
        }

        public static bool IsWithInLine(this BoardPosition position, PieceLine line)
        {
            return (position.X.WithInRang(line.StartPosition.X, line.EndPosition.X) ||
                    position.Y.WithInRang(line.StartPosition.Y, line.EndPosition.Y)) &&
                   IsOnLine(position, line);
        }

        public static bool IsOnLine(this BoardPosition position, PieceLine line)
        {
            return (position.X - line.StartPosition.X) * (position.Y - line.EndPosition.Y) ==
                   (position.X - line.EndPosition.X) * (position.Y - line.StartPosition.Y);
        }
    }
}
