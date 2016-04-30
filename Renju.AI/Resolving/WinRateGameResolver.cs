using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Practices.Unity;
using Renju.Core;
using Renju.Infrastructure.Execution;
using Renju.Infrastructure.Model;

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

        [Dependency]
        public GameOptions Options { get; set; }

        public int Depth { get; set; } = 5;

        public int Width { get; set; } = 4;

        public CancellationToken CancelTaken { get; set; }

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
                RaiseEvent(ResolvingBoard, new ResolvingBoardEventArgs(null));
                RaiseFinishedEvent();
            }
        }

        private double GetWinRateOf(IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point, Side side, int depth)
        {
            if (depth > Depth)
                return 0;

            if (CancelTaken.IsCancellationRequested)
                return 0;

            if (Options.IsAITimeLimited &&
                (ExecutionTimer.CurrentExecutionTime > Options.AIStepTimeSpan ||
                 ExecutionTimer.TotalExecutionTime > Options.AITotalTimeSpan))
            {
                Trace.TraceWarning(String.Format("Exeeded max step time of {0} or max time of {1}", Options.AIStepTimeSpan, Options.AITotalTimeSpan));
                return 0;
            }

            iteratedBoardCount++;
            var virtualBoard = board.With(point);
            var oppositeSide = Sides.Opposite(point.Status.Value);
            var drops = SelectDropsWithinWidth(virtualBoard, oppositeSide).ToList();
            RaiseEvent(ResolvingBoard, new ResolvingBoardEventArgs(virtualBoard));
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

        protected virtual IEnumerable<IReadOnlyBoardPoint> SelectDropsWithinWidth(IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            return _selector.SelectDrops(board, side).Where((p, i) => i < Width);
        }
    }
}
