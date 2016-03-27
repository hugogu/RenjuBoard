using System;

namespace Renju.Core
{
    public interface IAIPlayground
    {
        event EventHandler<PieceDropEventArgs> PieceDropped;

        Side? ExpectedNextTurn { get; }

        DropResult Drop(IReadOnlyBoardPoint point);
    }
}
