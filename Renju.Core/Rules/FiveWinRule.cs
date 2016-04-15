using System.Linq;

namespace Renju.Core.Rules
{
    public class FiveWinRule : WinRule
    {
        public override string Name
        {
            get { return "Win"; }
        }

        public override bool? Win(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return board[drop].GetRowsOnBoard(board).Any(line => line.Length > 4);
        }
    }
}
