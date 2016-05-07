using System;
using System.Collections.Generic;
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
                                   () => board.GetFours(side).SelectMany(l => l.GetBlockPoints(board)),
                                   () => board.GetFours(Sides.Opposite(side)).SelectMany(l => l.GetBlockPoints(board)),
                                   () => board.GetOpenThrees(side).SelectMany(l => l.GetBlockPoints(board)),
                                   () => board.GetOpenThrees(Sides.Opposite(side)).SelectMany(l => l.GetBlockPoints(board)),
                                   () => board.Points);

            return OrderCandidatesPointsByWeight(prioritizedDrops, board, side);
        }

        private IEnumerable<IReadOnlyBoardPoint> OrderCandidatesPointsByWeight(IEnumerable<IReadOnlyBoardPoint> pointsCandidates, IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            foreach (var weightedPoint in from point in pointsCandidates
                                          where point.Status == null && point.RequiresReevaluateWeight
                                          let drop = new PieceDrop(point.Position, side)
                                          where board.RuleEngine.CanDropOn(board, drop)
                                          let lines = point.GetRowsOnBoard(board, true)
                                          let pointWithWeight = new { Point = point, Weight = lines.Sum(_ => _.Weight) }
                                          orderby pointWithWeight.Weight descending, RandomSeed()
                                          select pointWithWeight)
            {
                weightedPoint.Point.RequiresReevaluateWeight = false;
                weightedPoint.Point.Weight = weightedPoint.Weight;
                yield return weightedPoint.Point;
            }
        }

        private IEnumerable<IReadOnlyBoardPoint> FindDropsFromPrioritizedTargets(IReadBoardState<IReadOnlyBoardPoint> board, params Func<IEnumerable<IReadOnlyBoardPoint>>[] findPointsFuncs)
        {
            foreach (var points in findPointsFuncs.Select(f => f().ToList()))
                if (points.Count > 0)
                    return points;

            return new IReadOnlyBoardPoint[0];
        }

        private int RandomSeed()
        {
            return RandomEqualSelections ? _random.Next() : 0;
        }
    }
}
