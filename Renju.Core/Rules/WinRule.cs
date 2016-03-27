namespace Renju.Core.Rules
{
    public abstract class WinRule : IGameRule
    {
        public abstract string Name { get; }

        public bool? CanDropOn(IReadBoardState board, PieceDrop drop)
        {
            return null;
        }

        public Side? NextSide(IReadBoardState board)
        {
            return null;
        }

        public abstract bool? Win(IReadBoardState board, PieceDrop drop);
    }
}
