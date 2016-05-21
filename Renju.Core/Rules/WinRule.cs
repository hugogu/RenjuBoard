namespace Renju.Core.Rules
{
    using System;
    using Infrastructure.Model;

    [Serializable]
    public abstract class WinRule : IGameRule
    {
        public abstract string Name { get; }

        public bool IsOptional
        {
            get { return false; }
        }

        public bool IsEnabled
        {
            get { return true; }
            set { throw new InvalidOperationException("This rule can't be disabled."); }
        }

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
