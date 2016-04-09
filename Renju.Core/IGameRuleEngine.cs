using System.Collections.Generic;

namespace Renju.Core
{
    public interface IGameRuleEngine
    {
        IEnumerable<IGameRule> ApplicableRules { get; }

        DropResult ProcessDrop(IGameBoard<IReadOnlyBoardPoint> board, PieceDrop drop);

        bool CanDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop);

        Side? IsWin(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop);
    }
}
