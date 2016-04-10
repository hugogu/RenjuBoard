using System;
using System.Collections.Generic;
using Renju.Core;
using Renju.Infrastructure.Execution;

namespace Renju.AI
{
    public interface IDropResolver : IReportExecutionStatus
    {
        int Depth { get; set; }

        int Width { get; set; }

        event EventHandler<ResolvingBoardEventArgs> ResolvingBoard;

        IEnumerable<IReadOnlyBoardPoint> Resolve(IGameBoard<IReadOnlyBoardPoint> board, Side side);
    }
}
