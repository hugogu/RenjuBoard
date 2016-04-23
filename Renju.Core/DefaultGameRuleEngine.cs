using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Renju.Infrastructure.Model;

namespace Renju.Core
{
    public class DefaultGameRuleEngine : IGameRuleEngine
    {
        private readonly IEnumerable<IGameRule> _rules;

        public DefaultGameRuleEngine(IEnumerable<IGameRule> rules)
        {
            if (rules == null)
                throw new ArgumentNullException("rules");
            Debug.Assert(rules.Any());
            _rules = rules;
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
            foreach (var rule in _rules)
            {
                var win = rule.Win(board, drop);
                if (win.HasValue && win.Value)
                {
                    return rule;
                }
            }
            return null;
        }

        protected virtual IGameRule GetRuleStopDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            foreach (var rule in _rules)
            {
                var canDrop = rule.CanDropOn(board, drop);
                if (canDrop.HasValue && canDrop == false)
                {
                    return rule;
                }
            }
            return null;
        }
    }
}
