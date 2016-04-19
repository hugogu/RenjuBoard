using System;
using System.Collections.Generic;
using System.Threading;
using Renju.Infrastructure.Execution;
using Renju.Infrastructure.Model;

namespace Renju.AI
{
    public interface IDropResolver : IReportExecutionStatus
    {
        int Depth { get; set; }

        int Width { get; set; }

        TimeSpan MaxStepTime { get; set; }

        TimeSpan MaxTotalTime { get; set; }

        CancellationToken CancelTaken { get; set; }

        event EventHandler<ResolvingBoardEventArgs> ResolvingBoard;

        IEnumerable<IReadOnlyBoardPoint> Resolve(IGameBoard<IReadOnlyBoardPoint> board, Side side);
    }
}
