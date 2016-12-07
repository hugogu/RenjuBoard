namespace Renju.Infrastructure.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Events;

    [ContractClass(typeof(ReadGameBoardContract<>))]
    public interface IReadBoardState<out TPoint>
        where TPoint : IReadOnlyBoardPoint
    {
        event EventHandler<GenericEventArgs<NewGameOptions>> Begin;

        event EventHandler<PieceDropEventArgs> PieceDropped;

        event EventHandler<BoardPosition> Taken;

        IGameRuleEngine RuleEngine { get; }

        int Size { get; }

        IEnumerable<TPoint> DroppedPoints { get; }

        IEnumerable<TPoint> Points { get; }

        IEnumerable<PieceLine> Lines { get; }

        TPoint this[BoardPosition position] { get; }
    }
}
