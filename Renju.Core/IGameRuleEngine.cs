using System.Collections.Generic;

namespace Renju.Core
{
    public interface IGameRuleEngine
    {
        IEnumerable<IGameRule> ApplicableRules { get; }

        DropResult ProcessDrop(GameBoard board, PieceDrop drop);

        bool CanDropOn(GameBoard board, PieceDrop drop);
    }
}
