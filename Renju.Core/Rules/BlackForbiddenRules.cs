using System;
using System.Linq;
using Renju.Infrastructure.Model;
using Renju.Infrastructure.Model.Extensions;

namespace Renju.Core.Rules
{
    [Serializable]
    public class BlackForbiddenRules : DropValidationRule
    {
        public override string Name
        {
            get { return "Black Forbidden"; }
        }

        public override bool? CanDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            if (drop.Side == Side.White)
                return true;

            var longLines = (from line in board.GetRowsFromPoint(board[drop], true)
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
    }
}
