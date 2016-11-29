namespace Renju.Infrastructure.AI
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;
    using Execution;
    using Model;

    [ContractClass(typeof(DropResolverContract))]
    public interface IDropResolver : IReportExecutionStatus
    {
        CancellationToken CancelTaken { get; set; }

        Task<IReadOnlyBoardPoint> ResolveAsync(IGameBoard<IReadOnlyBoardPoint> board, Side side);
    }
}
