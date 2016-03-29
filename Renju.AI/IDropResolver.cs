using System.Collections.Generic;
using Renju.Core;

namespace Renju.AI
{
    public interface IDropResolver
    {
        IEnumerable<IReadOnlyBoardPoint> Resolve(IGameBoard board, Side side);

        int Depth { get; set; }

        int Width { get; set; }
    }
}
