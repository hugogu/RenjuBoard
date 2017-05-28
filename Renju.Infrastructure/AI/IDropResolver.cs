namespace Renju.Infrastructure.AI
{
    using System.Threading;
    using System.Threading.Tasks;
    using Execution;
    using Model;

    public interface IDropResolver : IReportExecutionStatus
    {
        CancellationToken CancelTaken { get; set; }

        Task<IReadOnlyBoardPoint> ResolveAsync(IGameBoard<IReadOnlyBoardPoint> board, Side side);
    }
}
