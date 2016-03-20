using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renju.Core
{
    public static class GameBoardUtils
    {
        public static IEnumerable<PieceLine> GetLinesOnBoard(this BoardPoint point, GameBoard board, bool includeBlank = false)
        {
            foreach(var direction in GetAllDirections())
            {
                var endPosition = point.Position;

                do
                {
                    endPosition = endPosition + direction;
                } while (endPosition.IsOnBoard(board) &&
                         (Equals(board[endPosition].Status, point.Status) ||
                             (includeBlank && board[endPosition].Status == null)));

                endPosition = endPosition - direction;

                if (!Equals(endPosition, point.Position))
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
