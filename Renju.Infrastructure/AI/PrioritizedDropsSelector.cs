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

        public virtual IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side)
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
                () => board.Points.Where(p => p.Status == null).OrderByDescending(p => p.Weight));

            return OrderCandidatesPointsByWeight(prioritizedDrops, board, side);
        }

        public IDictionary<IReadOnlyBoardPoint, int> EvaluateWeight(IReadBoardState<IReadOnlyBoardPoint> board, IEnumerable<IReadOnlyBoardPoint> points, Side nextSide)
        {
            var originalWeight = new Dictionary<IReadOnlyBoardPoint, int>();
            foreach (var point in points)
            {
                originalWeight.Add(point, point.Weight);
                point.Weight = Convert.ToInt32(CacluateWeightOfPoint(board, point, nextSide));
            }

            return originalWeight;
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
            return findPointsFuncs.SelectMany(f => f().ToList()).Distinct()
                                  .Where(p => p.Status == null && board.RuleEngine.GetRuleStopDropOn(board, new PieceDrop(p.Position, side)) == null);
        }
    }
}
