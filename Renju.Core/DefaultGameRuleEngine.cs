using System;
using System.Collections.Generic;
using System.Linq;

namespace Renju.Core
{
    public class DefaultGameRuleEngine : IGameRuleEngine
    {
        private IEnumerable<IGameRule> _rules;

        public DefaultGameRuleEngine(IEnumerable<IGameRule> rules)
        {
            _rules = rules;
        }

        public IEnumerable<IGameRule> ApplicableRules
        {
            get { return _rules; }
        }

        public bool CanDropOn(IReadBoardState board, PieceDrop drop)
        {
            return GetRuleStopDropOn(board, drop) == null;
        }

        public DropResult ProcessDrop(IGameBoard board, PieceDrop drop)
        {
            var point = board[drop.X, drop.Y];
            if (point.Status != null)
                return DropResult.InvalidDrop;

            var notDroppingRule = GetRuleStopDropOn(board, drop);
            if (notDroppingRule != null)
                throw new InvalidOperationException(String.Format("Can't drop on {0} according to rule {1}", drop, notDroppingRule.Name));

            board.SetState(point.Position, drop.Side);
            board.SetIndex(point.Position, board.Drops.Count() + 1);
            foreach(var rule in _rules)
            {
                var win = rule.Win(board, drop);
                if (win.HasValue && win.Value)
                {
                    return DropResult.Win(drop.Side);
                }
            }
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

        public IGameRule GetRuleStopDropOn(IReadBoardState board, PieceDrop drop)
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
