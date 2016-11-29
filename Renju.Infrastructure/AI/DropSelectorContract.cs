namespace Renju.Infrastructure.AI
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Model;

    [ContractClassFor(typeof(IDropSelector))]
    public abstract class DropSelectorContract : IDropSelector
    {
        IEnumerable<IReadOnlyBoardPoint> IDropSelector.SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            Contract.Requires(board != null);
            Contract.Ensures(Contract.Result<IEnumerable<IReadOnlyBoardPoint>>() != null);

            return default(IEnumerable<IReadOnlyBoardPoint>);
        }
    }
}
