namespace Renju.Infrastructure.AI
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Model;

    public class WeightPatternDropSelector : PrioritizedDropsSelector
    {
        [Dependency]
        public IDropWeightEvaluator Weighter { get; set; }

        public override IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            return board.Points.Where(p => p.Status == null).OrderByDescending(p => p.Weight);
        }

        protected override double CacluateWeightOfPoint(IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint drop, Side dropSide)
        {
            return Weighter.CalculateWeight(board, drop, dropSide);
        }
    }
}
