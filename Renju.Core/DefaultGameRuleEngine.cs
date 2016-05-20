using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.Unity.Utility;
using Renju.Infrastructure.Model;

namespace Renju.Core
{
    [Serializable]
    public class DefaultGameRuleEngine : IGameRuleEngine
    {
        private readonly IEnumerable<IGameRule> _rules;

        public DefaultGameRuleEngine(IEnumerable<IGameRule> rules)
        {
            Guard.ArgumentNotNull(rules, "rules");
            Debug.Assert(rules.Any());
            _rules = rules;
            foreach(var rule in rules)
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

        public DropResult ProcessDrop(IGameBoard<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            var point = board[drop];
            if (point.Status != null)
                return DropResult.InvalidDrop;

            var notDroppingRule = GetRuleStopDropOn(board, drop);
            if (notDroppingRule != null)
                throw new InvalidOperationException(String.Format("Can't drop on {0} according to rule \"{1}\"", drop, notDroppingRule.Name));

            board.SetState(point.Position, drop.Side);
            board.SetIndex(point.Position, board.Points.Count(p => p.Index.HasValue) + 1);
            var winOnRule = GetRuleWin(board, drop);
            if (winOnRule != null)
                return DropResult.Win(drop.Side);

            foreach (var rule in _rules)
            {
                var next = rule.NextSide(board);
                if (next.HasValue)
                {
                    return DropResult.NoWin(next.Value);
                }
            }

            return DropResult.NoWin(Sides.Opposite(drop.Side));
        }

        protected virtual IGameRule GetRuleWin(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return _rules.FirstOrDefault(rule => rule.Win(board, drop) == true);
        }

        protected virtual IGameRule GetRuleStopDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return _rules.FirstOrDefault(rule => rule.CanDropOn(board, drop) == false);
        }
    }
}
