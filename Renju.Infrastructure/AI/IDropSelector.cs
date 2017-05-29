namespace Renju.Infrastructure.AI
{
    using System.Collections.Generic;
    using Model;

    public interface IDropSelector
    {
        IDictionary<IReadOnlyBoardPoint, int> EvaluateWeight(IReadBoardState<IReadOnlyBoardPoint> board, IEnumerable<IReadOnlyBoardPoint> points, Side nextSide);

        IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side);
    }
}
