using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Renju.Infrastructure;
using Renju.Infrastructure.Model;

namespace Renju.AI.Weights
{
    public class WeightedDropSelector : IDropSelector
    {
        private readonly Random _random;

        public WeightedDropSelector()
        {
            _random = new Random();
        }

        public bool RandomEqualSelections { get; set; } = true;

        public IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            var prioritizedDrops = FindDropsFromPrioritizedTargets(board,
                                   () => board.GetFours(side).ToList(),
                                   () => board.GetFours(Sides.Opposite(side)).ToList(),
                                   () => board.GetOpenThrees(side).ToList(),
                                   () => board.GetOpenThrees(Sides.Opposite(side)).ToList()).ToList();
            if (prioritizedDrops.Count > 0)
            {
                foreach (var drop in prioritizedDrops)
                    yield return drop;
                yield break;
            }

            var myThrees = board.GetThrees(side).Where(t => t.IsClosed(board));
            foreach (var myThree in myThrees)
            {
                foreach (var fourPoint in myThree.GetBlockPoints(board))
                    yield return fourPoint;
            }

            foreach (var weightedPoint in (from point in board.Points
                                           where point.Status == null && point.RequiresReevaluateWeight
                                           let drop = new PieceDrop(point.Position, side)
                                           where board.RuleEngine.CanDropOn(board, drop)
                                           let lines = point.GetRowsOnBoard(board, true)
                                           let weightedPoint = new { Point = point, Weight = lines.Sum(_ => _.Weight) }
                                           orderby weightedPoint.Weight descending, RandomSeed()
                                           group weightedPoint by weightedPoint.Weight >= 1000).Where(g => g.Any()).First())
            {
                weightedPoint.Point.RequiresReevaluateWeight = false;
                weightedPoint.Point.Weight = weightedPoint.Weight;
                yield return weightedPoint.Point;
            }
        }

        private IEnumerable<IReadOnlyBoardPoint> FindDropsFromPrioritizedTargets(IReadBoardState<IReadOnlyBoardPoint> board, params Func<ICollection<PieceLine>>[] findLinesFuncs)
        {
            foreach (var findLines in findLinesFuncs)
            {
                var lines = findLines();
                if (lines.Count > 0)
                {
                    foreach (var line in lines)
                    {
                        var drops = line.GetBlockPoints(board).ToList();
                        Debug.Assert(drops.Count > 0);
                        foreach (var drop in drops)
                            yield return drop;
                    }
                    yield break;
                }
            }
        }

        private int RandomSeed()
        {
            return RandomEqualSelections ? _random.Next() : 0;
        }
    }
}
