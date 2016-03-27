namespace Renju.Core
{
    public interface IGameRule
    {
        string Name { get; }

        bool? CanDropOn(IReadBoardState board, PieceDrop drop);

        bool? Win(IReadBoardState board, PieceDrop drop);

        Side? NextSide(IReadBoardState board);
    }
}
