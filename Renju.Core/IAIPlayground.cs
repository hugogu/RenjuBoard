namespace Renju.Core
{
    public interface IAIPlayground
    {
        Side? ExpectedNextTurn { get; }

        DropResult Drop(BoardPosition position, OperatorType type);
    }
}
