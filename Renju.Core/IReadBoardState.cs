using System;
using System.Collections.Generic;

namespace Renju.Core
{
    public interface IReadBoardState<out TPoint>
        where TPoint : IReadOnlyBoardPoint
    {
        TPoint this[BoardPosition position] { get; }

        event EventHandler<PieceDropEventArgs> PieceDropped;

        IGameRuleEngine RuleEngine { get; }

        int Size { get; }

        int DropsCount { get; }

        IEnumerable<TPoint> DroppedPoints { get; }

        IEnumerable<TPoint> Points { get; }

        IEnumerable<PieceLine> Lines { get; }

        bool IsDropped(BoardPosition position);
    }
}
