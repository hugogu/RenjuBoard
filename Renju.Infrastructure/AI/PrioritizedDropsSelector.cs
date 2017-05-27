namespace Renju.Infrastructure.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Events;
    using Infrastructure;
    using Microsoft.Practices.Unity;
    using Model;
    using Model.Extensions;
    using Prism.Events;

    public class PrioritizedDropsSelector : IDropSelector
    {
        public bool RandomEqualSelections { get; set; } = true;

        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        public IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            var prioritizedDrops = FindDropsFromPrioritizedTargets(
                board,
                side,
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

        protected virtual double CacluateWeightOfPoint(IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint drop, Side dropSide)
        {
            return board.GetRowsFromPoint(drop, true).Sum(_ => _.Weight);
        }

        private IEnumerable<IReadOnlyBoardPoint> OrderCandidatesPointsByWeight(IEnumerable<IReadOnlyBoardPoint> pointsCandidates, IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            foreach (var pointToWeight in pointsCandidates.Where(p => p.Status == null && p.RequiresReevaluateWeight))
            {
                pointToWeight.RequiresReevaluateWeight = false;
                pointToWeight.Weight = Convert.ToInt32(CacluateWeightOfPoint(board, pointToWeight, side));
                EventAggregator.GetEvent<EvaluatedPointEvent>().Publish(pointToWeight);
            }

            return from point in pointsCandidates
                   orderby point.Weight descending, RandomEqualSelections ? NumberUtils.NewRandom() : 0
                   select point;
        }

        private IEnumerable<IReadOnlyBoardPoint> FindDropsFromPrioritizedTargets(IReadBoardState<IReadOnlyBoardPoint> board, Side side, params Func<IEnumerable<IReadOnlyBoardPoint>>[] findPointsFuncs)
        {
            return findPointsFuncs.SelectMany(f => f().ToList())
                                  .Where(p => p.Status == null && board.RuleEngine.GetRuleStopDropOn(board, new PieceDrop(p.Position, side)) == null);
        }
    }
}
