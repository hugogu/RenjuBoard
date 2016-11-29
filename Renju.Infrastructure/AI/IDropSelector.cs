namespace Renju.Infrastructure.AI
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Model;

    [ContractClass(typeof(DropSelectorContract))]
    public interface IDropSelector
    {
        IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side);
    }
}
