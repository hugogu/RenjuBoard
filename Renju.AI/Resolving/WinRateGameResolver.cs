using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Renju.Core;

namespace Renju.AI.Resolving
{
    public class WinRateGameResolver : IDropResolver
    {
        private IDropSelector _selector;
        public WinRateGameResolver(IDropSelector selector)
        {
            _selector = selector;
        }

        public int Depth { get; set; } = 3;

        public int Width { get; set; } = 5;

        public IEnumerable<IReadOnlyBoardPoint> Resolve(IGameBoard board, Side side)
        {
            var rates = (from point in _selector.SelectDrops(board, side).Where((p, i) => i < Width).Select((p, i) => new { Point = p, Priority = i })
                        let virtualDrop = point.Point.As(side, board)
                        let pointWithRate = new { Point = point.Point, WinRate = GetWinRateOf(board, virtualDrop, side, 1) }
                        orderby pointWithRate.WinRate descending, point.Priority
                        select pointWithRate).ToList();

            rates.ForEach(p => Debug.WriteLine("{0}:{1}", p.Point, p.WinRate));

            return rates.Select(p => p.Point);
        }

        private double GetWinRateOf(IReadBoardState board, IReadOnlyBoardPoint point, Side side, int depth)
        {
            var virtualBoard = board.With(point);
            var oppositeSide = Sides.Opposite(point.Status.Value);
            var winSide = board.RuleEngine.IsWin(virtualBoard, new PieceDrop(point.Position.X, point.Position.Y, point.Status.Value));
            if (winSide.HasValue)
                return point.Status.Value == side ? 1.0 : -1.0;

            if (depth > Depth)
                return 0;

            var dropsWinRate = from drop in _selector.SelectDrops(virtualBoard, oppositeSide).Where((p, i) => i < Width)
                               let virtualDrop = drop.As(oppositeSide, virtualBoard)
                               select new { Point = virtualDrop, WinRate = GetWinRateOf(virtualBoard, virtualDrop, side, depth + 1) };

            return dropsWinRate.Sum(pair => pair.WinRate);
        }
    }
}
