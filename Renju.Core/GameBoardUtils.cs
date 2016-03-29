using System;
using System.Collections.Generic;
using System.Linq;

namespace Renju.Core
{
    public static class GameBoardUtils
    {
        public static IEnumerable<PieceLine> GetLinesOnBoard(this IReadOnlyBoardPoint point, IReadBoardState board, bool includeBlank = false)
        {
            foreach(var direction in GetHalfDirections())
            {
                var line = GetLineOnBoard(point.Position, board, direction, includeBlank);
                var oppositeLine = GetLineOnBoard(point.Position, board, direction.GetOpposite(), includeBlank);
                var combinedLine = line + oppositeLine;
                if (combinedLine != null)
                {
                    yield return combinedLine;
                    continue;
                }
                if (line != null)
                    yield return line;
                if (oppositeLine != null)
                    yield return oppositeLine;
            }
        }

        public static PieceLine GetLineOnBoard(this BoardPosition position, IReadBoardState board, BoardPosition direction, bool includeBlank)
        {
            return includeBlank ? position.GetDashLineOnBoard(board, direction) : position.GetContinuousLineOnBoard(board, direction);
        }

        public static PieceLine GetContinuousLineOnBoard(this BoardPosition position, IReadBoardState board, BoardPosition direction)
        {
            if (!board[position].Status.HasValue)
                throw new InvalidOperationException("ContinousLine can't start with blank.");

            var endPosition = position + direction;
            while(endPosition.IsOnBoard(board) && board[endPosition].Status.HasValue && board[endPosition].Status.Value == board[position].Status.Value)
            {
                endPosition += direction;
            }
            endPosition -= direction;

            return Equals(position, endPosition) ? null : new PieceLine(board, position, endPosition);
        }

        public static PieceLine GetDashLineOnBoard(this BoardPosition position, IReadBoardState board, BoardPosition direction)
        {
            var firstState = board[position].Status;
            var endPosition = position;
            while(endPosition.CanMoveAlone(board, direction, ref firstState))
            {
                endPosition += direction;
            }

            return (Equals(endPosition, position) || firstState == null) ?
                    null : new PieceLine(board, position, endPosition).TrimEnd();
        }

        public static bool IsOnBoard(this BoardPosition position, IReadBoardState board)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < board.Size && position.Y < board.Size;
        }

        public static BoardPosition GetOpposite(this BoardPosition position)
        {
            return new BoardPosition(-position.X, -position.Y);
        }

        public static IEnumerable<BoardPosition> GetHalfDirections()
        {
            yield return new BoardPosition(1, 0);
            yield return new BoardPosition(1, 1);
            yield return new BoardPosition(0, 1);
            yield return new BoardPosition(-1, 1);
        }

        public static IReadBoardState With(this IReadBoardState board, IReadOnlyBoardPoint point)
        {
            return new GameBoardDecoration(board, point);
        }

        public static IReadOnlyBoardPoint As(this IReadOnlyBoardPoint point, Side side, IReadBoardState board)
        {
            return new VirtualBoardPoint(point, side, board.Points.Where(p => p.Index.HasValue).Max(p => p.Index.Value) + 1);
        }

        private static bool CanMoveAlone(this BoardPosition position, IReadBoardState board, BoardPosition direction, ref Side? pickedSide)
        {
            var nextPosition = position + direction;
            if (!nextPosition.IsOnBoard(board))
                return false;

            if (!board[nextPosition].Status.HasValue)
                return true;

            if (pickedSide == null)
            {
                pickedSide = board[nextPosition].Status.Value;
                return true;
            }

            return pickedSide.Value == board[nextPosition].Status.Value;
        }
    }
}
