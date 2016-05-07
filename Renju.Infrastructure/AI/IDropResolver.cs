using System.Threading;
using System.Threading.Tasks;
using Renju.Infrastructure.Execution;
using Renju.Infrastructure.Model;

namespace Renju.Infrastructure.AI
{
    public interface IDropResolver : IReportExecutionStatus
    {
        int Depth { get; set; }

        int Width { get; set; }

        CancellationToken CancelTaken { get; set; }

        Task<IReadOnlyBoardPoint> ResolveAsync(IGameBoard<IReadOnlyBoardPoint> board, Side side);
    }
}
