namespace Renju.Core.Rules
{
    using System;
    using System.Linq;
    using Infrastructure.Model;
    using Infrastructure.Model.Extensions;

    [Serializable]
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
