using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Renju.Core;

namespace Renju.AI.Resolving
{
    public class WinRateGameResolver : IDropResolver
    {
        private IDropSelector _selector;
        private int iteratedBoardCount;

        public WinRateGameResolver(IDropSelector selector)
        {
            _selector = selector;
        }

        public int Depth { get; set; } = 5;

        public int Width { get; set; } = 4;

        public IEnumerable<IReadOnlyBoardPoint> Resolve(IGameBoard board, Side side)
        {
            var stopWatch = Stopwatch.StartNew();
            iteratedBoardCount = 0;
            foreach (var pointWithRate in from point in SelectDropsWithinWidth(board, side)
                                          let weight = point.Weight
                                          let winRate = GetWinRateOf(board, point.As(side, board), side, 1)
                                          orderby winRate descending, weight descending
                                          select new { Point = point, WinRate = winRate })
            {
                Debug.WriteLine("Evaluated {0} boards in {1} ms.", iteratedBoardCount, stopWatch.ElapsedMilliseconds);
                yield return pointWithRate.Point;
            }
        }

        private double GetWinRateOf(IReadBoardState board, IReadOnlyBoardPoint point, Side side, int depth)
        {
            if (depth > Depth)
                return 0;

            iteratedBoardCount++;
            var virtualBoard = board.With(point);
            var oppositeSide = Sides.Opposite(point.Status.Value);
            var winSide = board.RuleEngine.IsWin(virtualBoard, new PieceDrop(point.Position, point.Status.Value));
            if (winSide.HasValue)
            {
                Debug.WriteLine("Found a win path: " + String.Join("->", virtualBoard.DroppedPoints.Reverse()));
                return point.Status.Value == side ? 1.0 : -1.0;
            }

            var drops = SelectDropsWithinWidth(virtualBoard, oppositeSide).ToList();
            var winRate = (from drop in drops
                           let virtualDrop = drop.As(oppositeSide, virtualBoard)
                           select GetWinRateOf(virtualBoard, virtualDrop, side, depth + 1)).Sum() / drops.Count;

            if (depth == 1)
                Debug.WriteLine("{0}:{1},{2} Iteration: {3}", point, winRate, point.Weight, iteratedBoardCount);

            return winRate;
        }

        protected virtual IEnumerable<IReadOnlyBoardPoint> SelectDropsWithinWidth(IReadBoardState board, Side side)
        {
            return _selector.SelectDrops(board, side).Where((p, i) => i < Width);
        }
    }
}
