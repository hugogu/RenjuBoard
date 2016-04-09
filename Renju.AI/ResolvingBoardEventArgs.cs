using System;
using Renju.Core;

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
