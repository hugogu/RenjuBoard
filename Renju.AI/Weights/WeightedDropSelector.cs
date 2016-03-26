using System.Collections.Generic;
using System.Linq;
using Renju.Core;

namespace Renju.AI.Weights
{
    public class WeightedDropSelector : IDropSelector
    {
        public IEnumerable<BoardPoint> SelectDrops(GameBoard board, Side side)
        {
            foreach(var applicablePoint in from point in board.Points
                                           where point.Status == null
                                           let drop = new PieceDrop(point.Position.X, point.Position.Y, side)
                                           where board.RuleEngine.CanDropOn(board, drop)
                                           select point)
            {
                var linesFromPoint = applicablePoint.GetLinesOnBoard(board, true).Select(l => l.TrimEnd()).Where(l => l != null);
                applicablePoint.Weight = linesFromPoint.Sum(l => WeightLine(board, l, side) + WeightLine(board, l, Sides.Opposite(side)));
            }

            return board.Points.Where(p => p.Status == null).OrderByDescending(p => p.Weight);
        }

        private int WeightLine(GameBoard board, PieceLine line, Side nextSide)
        {
            if (line.DroppedCount >= 4)
                return board[line.EndPosition].Status == nextSide ? 1000 : 149;

            return line.DroppedCount * line.DroppedCount * 9 - line.Length;
        }
    }
}
