using System;
using System.Collections.Generic;

namespace Renju.Infrastructure.Model
{
    public interface IReadBoardState<out TPoint>
        where TPoint : IReadOnlyBoardPoint
    {
        TPoint this[BoardPosition position] { get; }

        event EventHandler<PieceDropEventArgs> PieceDropped;

        IGameRuleEngine RuleEngine { get; }

        int Size { get; }

        int DropsCount { get; }

        string VisualBoard { get; }

        IEnumerable<TPoint> DroppedPoints { get; }

        IEnumerable<TPoint> Points { get; }

        IEnumerable<PieceLine> Lines { get; }
    }
}
