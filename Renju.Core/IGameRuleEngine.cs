using System.Collections.Generic;

namespace Renju.Core
{
    public interface IGameRuleEngine
    {
        IEnumerable<IGameRule> ApplicableRules { get; }

        DropResult ProcessDrop(IGameBoard board, PieceDrop drop);

        bool CanDropOn(IReadBoardState board, PieceDrop drop);
    }
}
