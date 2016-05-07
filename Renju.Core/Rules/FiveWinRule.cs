using System.Linq;
using Renju.Infrastructure.Model;
using Renju.Infrastructure.Model.Extensions;

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
            return board.GetRowsFromPoint(board[drop]).Any(line => line.Length > 4);
        }
    }
}
