namespace Renju.Infrastructure.AI
{
    using Model;

    public interface IDropWeightEvaluator
    {
        double CalculateWeight(IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint drop, Side dropSide);
    }
}
