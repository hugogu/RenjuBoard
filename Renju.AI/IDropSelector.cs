using System.Collections.Generic;
using Renju.Core;

namespace Renju.AI
{
    public interface IDropSelector
    {
        IEnumerable<BoardPoint> SelectDrops(GameBoard board, Side side);
    }
}
