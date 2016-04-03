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
            foreach (var pointWithRate in from point in _selector.SelectDrops(board, side).Where((p, i) => i < Width)
                                          let virtualDrop = point.As(side, board)
                                          let winRate = GetWinRateOf(board, virtualDrop, side, 1)
                                          orderby winRate descending, point.Weight descending
                                          select new { Point = point, WinRate = winRate })
            {
                Debug.WriteLine("{0}:{1}", pointWithRate.Point, pointWithRate.WinRate);
                yield return pointWithRate.Point;
            }
        }

        private double GetWinRateOf(IReadBoardState board, IReadOnlyBoardPoint point, Side side, int depth)
        {
            var virtualBoard = board.With(point);
            var oppositeSide = Sides.Opposite(point.Status.Value);
            var winSide = board.RuleEngine.IsWin(virtualBoard, new PieceDrop(point.Position, point.Status.Value));
            if (winSide.HasValue)
                return point.Status.Value == side ? 1.0 : -1.0;

            if (depth > Depth)
                return 0;

            return (from drop in _selector.SelectDrops(virtualBoard, oppositeSide).Where((p, i) => i < Width)
                    let virtualDrop = drop.As(oppositeSide, virtualBoard)
                    select GetWinRateOf(virtualBoard, virtualDrop, side, depth + 1)).Sum();
        }
    }
}
