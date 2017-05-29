namespace Renju.Infrastructure.AI
{
    using Microsoft.Practices.Unity;
    using Model;

    public class WeightPatternDropSelector : PrioritizedDropsSelector
    {
        [Dependency]
        public IDropWeightEvaluator Weighter { get; set; }

        protected override double CacluateWeightOfPoint(IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint drop, Side dropSide)
        {
            return Weighter.CalculateWeight(board, drop, dropSide);
        }
    }
}
