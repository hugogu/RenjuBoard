using System.Linq;

namespace Renju.Core.Rules
{
    public class BlackForbiddenRules : IGameRule
    {
        public string Name
        {
            get { return "Black Forbidden"; }
        }

        public bool? CanDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            if (drop.Side == Side.White)
                return true;

            var longLines = (from line in board[drop].GetRowsOnBoard(board, true)
                             where line.Side == drop.Side && line.DroppedCount >= 2
                             select line).ToList();

            if (longLines.Any(l => l.DroppedCount > 4 && l.Length - l.DroppedCount == 1))
                return false;

            if (longLines.Count <= 1)
                return true;

            if (longLines.Count(l => !l.IsClosed(board) && l.DroppedCount == 2) >= 2)
                return false;

            return true;
        }

        public Side? NextSide(IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return null;
        }

        public bool? Win(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return null;
        }
    }
}
