namespace Renju.Infrastructure.AI
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Model;

    [ContractClassFor(typeof(IDropSelector))]
    public abstract class DropSelectorContract : IDropSelector
    {
        IEnumerable<IReadOnlyBoardPoint> IDropSelector.SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            Contract.Requires(board != null);
            Contract.Ensures(Contract.Result<IEnumerable<IReadOnlyBoardPoint>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<IReadOnlyBoardPoint>>().Any());

            return default(IEnumerable<IReadOnlyBoardPoint>);
        }
    }
}
