using System;
using System.Collections.Generic;

namespace Renju.Core
{
    public interface IReadBoardState
    {
        IReadOnlyBoardPoint this[BoardPosition position] { get; }

        event EventHandler<PieceDropEventArgs> PieceDropped;

        IGameRuleEngine RuleEngine { get; }

        int Size { get; }

        int DropsCount { get; }

        IEnumerable<IReadOnlyBoardPoint> Points { get; }

        bool IsDropped(BoardPosition position);
    }
}
