using System;
using Renju.Infrastructure;
using Renju.Infrastructure.Model;

namespace Renju.Core.Rules
{
    [Serializable]
    public class OpeningRules : DropValidationRule
    {
        public override string Name
        {
            get { return "Opening Rules"; }
        }

        public override bool? CanDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            if (board.DropsCount == 0)
                return drop.X == 7 && drop.Y == 7;
            if (board.DropsCount == 1)
                return drop.X.InRang(6, 8) && drop.Y.InRang(6, 8);
            if (board.DropsCount == 2)
                return drop.X.InRang(5, 9) && drop.Y.InRang(5, 9);

            return true;
        }
    }
}
