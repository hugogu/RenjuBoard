using Renju.Infrastructure.Model;

namespace Renju.Core.Rules
{
    public abstract class WinRule : IGameRule
    {
        public abstract string Name { get; }

        public bool? CanDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return null;
        }

        public Side? NextSide(IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return null;
        }

        public abstract bool? Win(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop);
    }
}
