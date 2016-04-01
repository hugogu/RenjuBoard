using System;
using System.Collections.Generic;

namespace Renju.Core
{
    public interface IReadBoardState
    {
        IReadOnlyBoardPoint this[int x, int y] { get; }

        IReadOnlyBoardPoint this[BoardPosition position] { get; }

        event EventHandler<PieceDropEventArgs> PieceDropped;

        IGameRuleEngine RuleEngine { get; }

        int Size { get; }

        IEnumerable<IReadOnlyBoardPoint> Points { get; }
    }
}
