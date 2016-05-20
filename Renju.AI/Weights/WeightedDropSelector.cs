using System;
using System.Collections.Generic;
using System.Linq;
using Renju.Infrastructure;
using Renju.Infrastructure.AI;
using Renju.Infrastructure.Model;
using Renju.Infrastructure.Model.Extensions;

namespace Renju.AI.Weights
{
    public class WeightedDropSelector : IDropSelector
    {
        public bool RandomEqualSelections { get; set; } = true;

        public IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            var prioritizedDrops = FindDropsFromPrioritizedTargets(board,
                                   () => board.GetLines(c => c >= 4, side, true).SelectMany(l => l.GetBlockPoints(board)),
                                   () => board.GetLines(c => c >= 4, Sides.Opposite(side), true).SelectMany(l => l.GetBlockPoints(board)),
                                   () => board.GetLines(c => c == 3, side, true).SelectMany(l => l.GetBlockPoints(board)),
                                   () => board.GetLines(c => c == 3, Sides.Opposite(side), true).SelectMany(l => l.GetBlockPoints(board)),
                                   () => board.GetLines(c => c == 2).SelectMany(l => l.GetContinousPoints(board)).Concat(
                                         board.GetLines(c => c == 3).Where(l => l.IsClosed(board)).SelectMany(l => l.GetBlockPoints(board))),
                                   () => board.IterateNearbyPointsOf(board.DroppedPoints.Last(), 2)
                                              .Where(p => p.Status == null)
                                              .OrderBy(p => p.To(board.DroppedPoints.Last(), board).Length));

            return OrderCandidatesPointsByWeight(prioritizedDrops, board, side);
        }

        private IEnumerable<IReadOnlyBoardPoint> OrderCandidatesPointsByWeight(IEnumerable<IReadOnlyBoardPoint> pointsCandidates, IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            foreach (var pointToWeight in pointsCandidates.Where(p => p.Status == null && p.RequiresReevaluateWeight))
            {
                pointToWeight.RequiresReevaluateWeight = false;
                pointToWeight.Weight = board.GetRowsFromPoint(pointToWeight, true).Sum(_ => _.Weight);
            }
            return from point in pointsCandidates
                   where point.Status == null && board.RuleEngine.CanDropOn(board, new PieceDrop(point.Position, side))
                   orderby point.Weight descending, RandomEqualSelections ? NumberUtils.NewRandom() : 0
                   select point;
        }

        private IEnumerable<IReadOnlyBoardPoint> FindDropsFromPrioritizedTargets(IReadBoardState<IReadOnlyBoardPoint> board, params Func<IEnumerable<IReadOnlyBoardPoint>>[] findPointsFuncs)
        {
            foreach (var points in findPointsFuncs.Select(f => f().ToList()))
                if (points.Count > 0)
                    return points;

            throw new InvalidOperationException("Can't find any point to drop. One side must have won the game.");
        }
    }
}
