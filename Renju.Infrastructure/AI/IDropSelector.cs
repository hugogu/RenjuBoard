namespace Renju.Infrastructure.AI
{
    using System.Collections.Generic;
    using Model;

    public interface IDropSelector
    {
        bool RandomEqualSelections { get; set; }

        IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side);
    }
}
