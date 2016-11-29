namespace Renju.Infrastructure.Model
{
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(GameBoardContract<>))]
    public interface IGameBoard<out TPoint> : IReadBoardState<TPoint>
        where TPoint : IReadOnlyBoardPoint
    {
        void BeginGame();

        Side? ExpectedNextTurn { get; }

        void Take(BoardPosition position);

        DropResult Drop(BoardPosition position, OperatorType operatorType);
    }
}
