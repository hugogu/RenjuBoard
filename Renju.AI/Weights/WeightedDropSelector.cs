using System.Collections.Generic;
using System.Linq;
using Renju.Core;

namespace Renju.AI.Weights
{
    public class WeightedDropSelector : IDropSelector
    {
        public IEnumerable<IReadOnlyBoardPoint> SelectDrops(IReadBoardState board, Side side)
        {
            foreach(var weightedPoint in (from point in board.Points
                                          where point.Status == null
                                          let drop = new PieceDrop(point.Position, side)
                                          where board.RuleEngine.CanDropOn(board, drop)
                                          let lines = point.GetLinesOnBoard(board, true)
                                          let weightedPoint = new { Point = point, Weight = lines.Sum(l => WeightLine(board, l, side)) }
                                          orderby weightedPoint.Weight descending
                                          group weightedPoint by weightedPoint.Weight >= 1000).First())
            {
                weightedPoint.Point.Weight = weightedPoint.Weight;
                yield return weightedPoint.Point;
            }
        }

        private int WeightLine(IReadBoardState board, PieceLine line, Side nextSide)
        {
            if (line.DroppedCount >= 4)
            {
                if (line.Length == 5)
                    return 1000;
                return 0;
            }
            else if (line.DroppedCount == 3)
            {
                if (line.IsEndClosed || line.IsStartClosed)
                {
                    return 40;
                }
                else
                {
                    if (line.Length == 4)
                        return 249;
                    if (line.Length == 5)
                        return 200;
                    return 0;
                }
            }
            else if (line.DroppedCount == 2)
            {
                if (line.Length == 3)
                {
                    return line.IsStartClosed || line.IsEndClosed ? 5 : 55;
                }
                if (line.Length == 4)
                {
                    return line.IsEndClosed || line.IsStartClosed ? 4 : 50;
                }
                if (line.Length == 5)
                {
                    return line.IsStartClosed || line.IsEndClosed ? 1 : 10;
                }
            }
            else if (line.DroppedCount == 1)
            {
                return 6 - line.Length;
            }
            return 0;
        }
    }
}
