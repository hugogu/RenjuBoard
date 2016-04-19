using System;
using Renju.Infrastructure.Model;

namespace Renju.AI
{
    public class ResolvingBoardEventArgs : EventArgs
    {
        public ResolvingBoardEventArgs(IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Board = board;
        }

        public IReadBoardState<IReadOnlyBoardPoint> Board { get; private set; }
    }
}
