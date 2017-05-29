namespace Renju.Infrastructure.Model
{
    public interface IGameBoard<out TPoint> : IReadBoardState<TPoint>
        where TPoint : IReadOnlyBoardPoint
    {
        void BeginGame();

        Side? ExpectedNextTurn { get; }

        void Take(BoardPosition position);

        DropResult Drop(BoardPosition position, OperatorType operatorType);
    }
}
