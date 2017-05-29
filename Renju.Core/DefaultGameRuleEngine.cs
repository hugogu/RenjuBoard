namespace Renju.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Infrastructure.Model;

    [Serializable]
    public class DefaultGameRuleEngine : IGameRuleEngine
    {
        private readonly IEnumerable<IGameRule> _rules;

        public DefaultGameRuleEngine(IEnumerable<IGameRule> rules)
        {
            Debug.Assert(rules != null);
            Debug.Assert(rules.Any());
            _rules = rules;
            foreach (var rule in rules)
                Trace.WriteLine("Loaded rule " + rule.Name);
        }

        public IEnumerable<IGameRule> ApplicableRules
        {
            get { return _rules; }
        }

        public bool CanDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return GetRuleStopDropOn(board, drop) == null;
        }

        public Side? IsWin(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return GetRuleWin(board, drop) != null ? drop.Side : (Side?)null;
        }

        public Side GetNextSide(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return _rules.Select(rule => rule.NextSide(board))
                         .FirstOrDefault(side => side.HasValue) ?? Sides.Opposite(drop.Side);
        }

        public virtual IGameRule GetRuleWin(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return _rules.FirstOrDefault(rule => rule.Win(board, drop) == true);
        }

        public virtual IGameRule GetRuleStopDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return _rules.FirstOrDefault(rule => rule.CanDropOn(board, drop) == false);
        }
    }
}
