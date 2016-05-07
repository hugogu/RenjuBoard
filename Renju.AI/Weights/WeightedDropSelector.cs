using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Renju.Infrastructure.AI;
using Renju.Infrastructure.Model;
using Renju.Infrastructure.Model.Extensions;

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
                foreach (var drop in OrderCandidatesPointsByWeight(prioritizedDrops, board, side))
                    yield return drop;
                yield break;
            }

            foreach (var weightedPoint in OrderCandidatesPointsByWeight(board.Points, board, side))
            {
                yield return weightedPoint;
            }
        }

        private IEnumerable<IReadOnlyBoardPoint> OrderCandidatesPointsByWeight(IEnumerable<IReadOnlyBoardPoint> pointsCandidates, IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            foreach (var weightedPoint in from point in pointsCandidates
                                          where point.Status == null && point.RequiresReevaluateWeight
                                          let drop = new PieceDrop(point.Position, side)
                                          where board.RuleEngine.CanDropOn(board, drop)
                                          let lines = board.GetRowsFromPoint(point, true)
                                          let pointWithWeight = new { Point = point, Weight = lines.Sum(_ => _.Weight) }
                                          orderby pointWithWeight.Weight descending, RandomSeed()
                                          select pointWithWeight)
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
