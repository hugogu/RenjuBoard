using System.Collections.Generic;

namespace Renju.Core
{
    public interface IGameBoard : IReadBoardState
    {
        IEnumerable<PieceDrop> Drops { get; }

        void SetState(BoardPosition position, Side side);

        void SetIndex(BoardPosition position, int index);
    }
}
