namespace Renju.Infrastructure.Model
{
    using System.Collections.Generic;

    public interface IGameRuleEngine
    {
        IEnumerable<IGameRule> ApplicableRules { get; }

        Side GetNextSide(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop);

        IGameRule GetRuleStopDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop);

        Side? IsWin(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop);
    }
}
