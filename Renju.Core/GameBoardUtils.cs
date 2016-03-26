using System.Collections.Generic;

namespace Renju.Core
{
    public static class GameBoardUtils
    {
        public static IEnumerable<PieceLine> GetLinesOnBoard(this BoardPoint point, GameBoard board, bool includeBlank = false)
        {
            foreach(var direction in GetAllDirections())
            {
                Side? endSide = point.Status;
                var endPosition = point.Position;
                do
                {
                    endPosition += direction;
                    if (!endPosition.IsOnBoard(board))
                    {
                        endPosition -= direction;
                        break;
                    }
                    var endPoint = board[endPosition];
                    if (endSide == null && endPoint.Status.HasValue)
                    {
                        includeBlank = false;
                        endSide = endPoint.Status;
                    }
                } while ((Equals(board[endPosition].Status, endSide) ||
                             (includeBlank && board[endPosition].Status == null)));

                if (!Equals(board[endPosition].Status, endSide))
                    endPosition -= direction;

                if (!Equals(endPosition, point.Position) && endSide.HasValue)
                    yield return new PieceLine(board, point.Position, endPosition);
            }
        }

        public static bool IsOnBoard(this BoardPosition position, GameBoard board)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < board.Size && position.Y < board.Size;
        }

        public static IEnumerable<BoardPosition> GetAllDirections()
        {
            yield return new BoardPosition(1, 0);
            yield return new BoardPosition(1, 1);
            yield return new BoardPosition(0, 1);
            yield return new BoardPosition(-1, 1);
            yield return new BoardPosition(-1, 0);
            yield return new BoardPosition(-1, -1);
            yield return new BoardPosition(0, -1);
            yield return new BoardPosition(1, -1);
        }
    }
}
