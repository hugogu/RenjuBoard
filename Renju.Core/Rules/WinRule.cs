namespace Renju.Core.Rules
{
    public abstract class WinRule : IGameRule
    {
        public abstract string Name { get; }

        public bool? CanDropOn(GameBoard board, PieceDrop drop)
        {
            return null;
        }

        public Side? NextSide(GameBoard board)
        {
            return null;
        }

        public abstract bool? Win(GameBoard board, PieceDrop drop);
    }
}
