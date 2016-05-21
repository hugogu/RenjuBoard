namespace Renju.AI.Resolving
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Practices.Unity;
    using Prism.Events;
    using Core;
    using Infrastructure.AI;
    using Infrastructure.Events;
    using Infrastructure.Execution;
    using Infrastructure.Model;

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

        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        public int Depth { get; set; } = 5;

        public int Width { get; set; } = 4;

        public CancellationToken CancelTaken { get; set; }

        public async Task<IReadOnlyBoardPoint> ResolveAsync(IGameBoard<IReadOnlyBoardPoint> board, Side side)
        {
            return await Task.Run(() => Resolve(board, side).First());
        }

        protected internal virtual void PublishResolvingBoardEvent(IReadBoardState<IReadOnlyBoardPoint> board)
        {
            if (EventAggregator == null)
                return;

            EventAggregator.GetEvent<ResolvingBoardEvent>().Publish(board);
        }

        protected virtual IEnumerable<IReadOnlyBoardPoint> Resolve(IGameBoard<IReadOnlyBoardPoint> board, Side side)
        {
            iteratedBoardCount = 0;
            try
            {
                RaiseStartedEvent();
                foreach (var pointWithRate in from point in SelectDropsWithinWidth(board, side)
                                              let weight = point.Weight
                                              let winRateWithPath = GetWinRateOf(board, point.As(side, board), side, 1)
                                              orderby winRateWithPath.WinRate descending, weight descending
                                              select new { Point = point, WinRate = winRateWithPath })
                {
                    Debug.Assert(pointWithRate.Point.Status == null, "A point candidate must be empty.");
                    Trace.WriteLine(String.Format("Evaluated {0} boards in {1} ms.", iteratedBoardCount, ExecutionTimer.CurrentExecutionTime.TotalMilliseconds));
                    yield return pointWithRate.Point;
                }
            }
            finally
            {
                PublishResolvingBoardEvent(null);
                RaiseFinishedEvent();
            }
        }

        protected virtual IEnumerable<IReadOnlyBoardPoint> SelectDropsWithinWidth(IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            return _selector.SelectDrops(board, side).Where((p, i) => i < Width);
        }

        private WinRateWithPath GetWinRateOf(IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point, Side side, int depth)
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
            var winSide = board.RuleEngine.IsWin(virtualBoard, new PieceDrop(point.Position, point.Status.Value));
            if (winSide.HasValue)
            {
                Trace.WriteLine("Found a win path: " + String.Join("->", virtualBoard.DroppedPoints.Reverse()));
                return new WinRateWithPath(point.Status.Value == side ? 1.0 : -1.0, virtualBoard.DroppedPoints.Reverse());
            }

            var oppositeSide = Sides.Opposite(point.Status.Value);
            var drops = SelectDropsWithinWidth(virtualBoard, oppositeSide).ToList();
            Debug.Assert(drops.Count > 0, "drop selector must yield some point candidate.");
            PublishResolvingBoardEvent(virtualBoard);
            RaiseStepFinishedEvent();
            var winRate = (from drop in drops
                           let virtualDrop = drop.As(oppositeSide, virtualBoard)
                           select GetWinRateOf(virtualBoard, virtualDrop, side, depth + 1).WinRate).Sum() / drops.Count;

            if (depth == 1)
                Trace.WriteLine(String.Format("{0}:{1},{2} Iteration: {3}", point, winRate, point.Weight, iteratedBoardCount));

            return new WinRateWithPath(winRate, virtualBoard.DroppedPoints.Reverse());
        }
    }
}
