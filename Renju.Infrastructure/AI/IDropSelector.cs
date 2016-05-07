using System.Collections.Generic;
using Renju.Infrastructure.Model;

namespace Renju.Infrastructure.AI
{
    public interface IDropSelector
    {
        IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side);

        bool RandomEqualSelections { get; set; }
    }
}
