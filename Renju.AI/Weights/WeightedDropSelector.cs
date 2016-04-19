using System;
using System.Collections.Generic;
using System.Linq;
using Renju.Infrastructure;
using Renju.Infrastructure.Model;

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

        public IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            var myFours = board.GetFours(side);
            foreach(var myFour in myFours)
            {
                foreach (var blockPoint in myFour.GetBlockPoints(board))
                {
                    yield return blockPoint;
                    yield break;
                }
            }
            var fours = board.GetFours(Sides.Opposite(side));
            if (fours.Any())
            {
                foreach (var blockPoint in fours.First().GetBlockPoints(board))
                {
                    yield return blockPoint;
                    yield break;
                }
            }

            var openThrees = board.GetOpenThrees(Sides.Opposite(side));
            if (openThrees.Any())
            {
                foreach(var openThree in openThrees)
                {
                    foreach(var blockPoint in openThree.GetBlockPoints(board))
                    {
                        yield return blockPoint;
                    }
                }
                var myThrees = board.GetThrees(side);
                foreach(var myThree in myThrees)
                {
                    foreach (var fourPoint in myThree.GetBlockPoints(board))
                        yield return fourPoint;
                }
                yield break;
            }

            foreach(var weightedPoint in (from point in board.Points
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

        private int RandomSeed()
        {
            return RandomEqualSelections ? _random.Next() : 0;
        }
    }
}
