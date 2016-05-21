namespace Renju.Infrastructure.AI
{
    using System.Collections.Generic;
    using Model;

    public interface IDropSelector
    {
        IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side);
    }
}
