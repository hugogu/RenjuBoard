namespace Renju.Core.Rules
{
    using System;
    using System.Linq;
    using Infrastructure;
    using Infrastructure.Model;

    [Serializable]
    public class OpeningRules : DropValidationRule
    {
        public override string Name
        {
            get { return "Opening Rules"; }
        }

        public override bool? CanDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            var count = board.DroppedPoints.Count();
            if (count == 0)
                return drop.X == 7 && drop.Y == 7;
            if (count == 1)
                return drop.X.InRang(6, 8) && drop.Y.InRang(6, 8);
            if (count == 2)
                return drop.X.InRang(5, 9) && drop.Y.InRang(5, 9);

            return true;
        }
    }
}
