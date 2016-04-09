namespace Renju.Core
{
    public interface IGameBoard<out TPoint> : IReadBoardState<TPoint>
        where TPoint : IReadOnlyBoardPoint
    {
        Side? ExpectedNextTurn { get; }

        void SetState(BoardPosition position, Side side);

        void SetIndex(BoardPosition position, int index);

        void Take(BoardPosition position);

        DropResult Drop(BoardPosition position, OperatorType operatorType);
    }
}
