using System;
using System.Collections.Generic;
using System.Linq;
using Renju.Core;

namespace Renju.AI.Weights
{
    public class WeightedDropSelector : IDropSelector
    {
        private Random _random;

        public WeightedDropSelector()
        {
            _random = new Random();
        }

        public bool RandomEqualSelections { get; set; }

        public IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState board, Side side)
        {
            foreach(var weightedPoint in (from point in board.Points
                                          where point.Status == null && point.RequiresReevaluateWeight
                                          let drop = new PieceDrop(point.Position, side)
                                          where board.RuleEngine.CanDropOn(board, drop)
                                          let lines = point.GetLinesOnBoard(board, true)
                                          let weightedPoint = new { Point = point, Weight = lines.Sum(_ => _.Weight) }
                                          orderby weightedPoint.Weight descending, RandomSeed()
                                          group weightedPoint by weightedPoint.Weight >= 1000).Where(g => g.Any()).First())
            {
                weightedPoint.Point.RequiresReevaluateWeight = false;
                weightedPoint.Point.Weight = weightedPoint.Weight;
                yield return weightedPoint.Point;
            }
        }

        private int RandomSeed()
        {
            return RandomEqualSelections ? _random.Next() : 0;
        }
    }
}
