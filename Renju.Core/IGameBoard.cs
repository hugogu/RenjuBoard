namespace Renju.Core
{
    public interface IGameBoard : IReadBoardState
    {
        Side? ExpectedNextTurn { get; }

        void SetState(BoardPosition position, Side side);

        void SetIndex(BoardPosition position, int index);

        void Take(BoardPosition position);

        DropResult Drop(BoardPosition position, OperatorType operatorType);
    }
}
