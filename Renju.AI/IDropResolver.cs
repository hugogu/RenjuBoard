using System;
using System.Collections.Generic;
using Renju.Core;

namespace Renju.AI
{
    public interface IDropResolver
    {
        int Depth { get; set; }

        int Width { get; set; }

        event EventHandler<ResolvingBoardEventArgs> ResolvingBoard;

        IEnumerable<IReadOnlyBoardPoint> Resolve(IGameBoard<IReadOnlyBoardPoint> board, Side side);
    }
}
