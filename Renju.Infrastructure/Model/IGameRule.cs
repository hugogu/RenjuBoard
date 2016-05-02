namespace Renju.Infrastructure.Model
{
    public interface IGameRule
    {
        string Name { get; }

        bool IsOptional { get; }

        bool? CanDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop);

        bool? Win(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop);

        Side? NextSide(IReadBoardState<IReadOnlyBoardPoint> board);
    }
}
