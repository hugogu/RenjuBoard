namespace Renju.Infrastructure.Model
{
    using System;
    using System.Collections.Generic;

    public interface IReadBoardState<out TPoint>
        where TPoint : IReadOnlyBoardPoint
    {
        event EventHandler<PieceDropEventArgs> PieceDropped;

        IGameRuleEngine RuleEngine { get; }

        int Size { get; }

        int DropsCount { get; }

        string VisualBoard { get; }

        IEnumerable<TPoint> DroppedPoints { get; }

        IEnumerable<TPoint> Points { get; }

        IEnumerable<PieceLine> Lines { get; }

        TPoint this[BoardPosition position] { get; }
    }
}
