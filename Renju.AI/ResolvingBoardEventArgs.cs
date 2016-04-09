using System;
using Renju.Core;

namespace Renju.AI
{
    public class ResolvingBoardEventArgs : EventArgs
    {
        public ResolvingBoardEventArgs(IReadBoardState board)
        {
            Board = board;
        }

        public IReadBoardState Board { get; private set; }
    }
}
