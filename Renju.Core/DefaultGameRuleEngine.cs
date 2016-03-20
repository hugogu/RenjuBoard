using System;
using System.Collections.Generic;

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

        public DropResult ProcessDrop(GameBoard board, PieceDrop drop)
        {
            var point = board[drop.X, drop.Y];
            if (point.Status != null)
                return DropResult.AlreadyDropped(drop.Side);

            foreach (var rule in _rules)
            {
                var canDrop = rule.CanDropOn(board, drop);
                if (canDrop.HasValue && canDrop == false)
                {
                    throw new InvalidOperationException(String.Format("Can't drop on {0} according to rule {1}", drop, rule.Name));
                }
            }
            point.Status = drop.Side;
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
    }
}
