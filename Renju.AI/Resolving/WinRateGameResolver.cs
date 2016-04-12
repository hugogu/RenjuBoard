using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Renju.Core;
using Renju.Infrastructure.Execution;

namespace Renju.AI.Resolving
{
    public class WinRateGameResolver : ReportExecutionObject, IDropResolver
    {
        private readonly IDropSelector _selector;
        private int iteratedBoardCount;

        public WinRateGameResolver(IDropSelector selector)
        {
            _selector = selector;
        }

        public int Depth { get; set; } = 5;

        public int Width { get; set; } = 4;

        public TimeSpan MaxStepTime { get; set; } = TimeSpan.FromSeconds(20);

        public TimeSpan MaxTotalTime { get; set; } = TimeSpan.FromSeconds(500);

        public event EventHandler<ResolvingBoardEventArgs> ResolvingBoard;

        public IEnumerable<IReadOnlyBoardPoint> Resolve(IGameBoard<IReadOnlyBoardPoint> board, Side side)
        {
            iteratedBoardCount = 0;
            try
            {
                RaiseStartedEvent();
                foreach (var pointWithRate in from point in SelectDropsWithinWidth(board, side)
                                              let weight = point.Weight
                                              let winRate = GetWinRateOf(board, point.As(side, board), side, 1)
                                              orderby winRate descending, weight descending
                                              select new { Point = point, WinRate = winRate })
                {
                    Debug.WriteLine("Evaluated {0} boards in {1} ms.", iteratedBoardCount, ExecutionTimer.CurrentExecutionTime.TotalMilliseconds);
                    yield return pointWithRate.Point;
                }
            }
            finally
            {
                RaiseResolvingBoardEvent(null);
                RaiseFinishedEvent();
            }
        }

        private double GetWinRateOf(IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point, Side side, int depth)
        {
            if (depth > Depth)
                return 0;

            if (ExecutionTimer.CurrentExecutionTime > MaxStepTime || ExecutionTimer.TotalExecutionTime > MaxTotalTime)
            {
                Trace.TraceWarning(String.Format("Exeeded max step time of {0} or max time of {1}", MaxStepTime, MaxTotalTime));
                return 0;
            }

            iteratedBoardCount++;
            var virtualBoard = board.With(point);
            var oppositeSide = Sides.Opposite(point.Status.Value);
            var drops = SelectDropsWithinWidth(virtualBoard, oppositeSide).ToList();
            RaiseResolvingBoardEvent(virtualBoard);
            RaiseStepFinishedEvent();
            var winSide = board.RuleEngine.IsWin(virtualBoard, new PieceDrop(point.Position, point.Status.Value));
            if (winSide.HasValue)
            {
                Debug.WriteLine("Found a win path: " + String.Join("->", virtualBoard.DroppedPoints.Reverse()));
                return point.Status.Value == side ? 1.0 : -1.0;
            }
            var winRate = (from drop in drops
                           let virtualDrop = drop.As(oppositeSide, virtualBoard)
                           select GetWinRateOf(virtualBoard, virtualDrop, side, depth + 1)).Sum() / drops.Count;

            if (depth == 1)
                Debug.WriteLine("{0}:{1},{2} Iteration: {3}", point, winRate, point.Weight, iteratedBoardCount);

            return winRate;
        }

        protected virtual void RaiseResolvingBoardEvent(IReadBoardState<IReadOnlyBoardPoint> board)
        {
            var tmp = ResolvingBoard;
            if (tmp != null)
            {
                tmp(this, new ResolvingBoardEventArgs(board));
            }
        }

        protected virtual IEnumerable<IReadOnlyBoardPoint> SelectDropsWithinWidth(IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            return _selector.SelectDrops(board, side).Where((p, i) => i < Width);
        }
    }
}
