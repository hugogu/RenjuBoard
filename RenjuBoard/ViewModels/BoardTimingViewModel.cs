namespace RenjuBoard.ViewModels
{
    using System;
    using System.Reactive.Linq;
    using Renju.Infrastructure;
    using Renju.Infrastructure.AI;
    using Renju.Infrastructure.Execution;
    using Renju.Infrastructure.Model;

    public class BoardTimingViewModel : DisposableModelBase
    {
        private readonly ExecutionTimer _blackTimer;
        private readonly ExecutionTimer _whiteTimer;

        // TODO: Support two AI and zero AI.
        public BoardTimingViewModel(GameOptions options, IGameBoard<IReadOnlyBoardPoint> gameBoard, IDropResolver ai)
        {
            var humanTimer = new SideExecutionReporter(gameBoard, options.AIFirst ? Side.White : Side.Black);
            _blackTimer = options.AIFirst ? ai.ExecutionTimer : humanTimer.ExecutionTimer;
            _whiteTimer = options.AIFirst ? humanTimer.ExecutionTimer : ai.ExecutionTimer;
            AutoDispose(humanTimer);
            AutoDispose(_blackTimer.ObserveProperty(() => _blackTimer.TotalExecutionTime)
                                   .Subscribe(_ => OnPropertyChanged(() => BlackTime)));
            AutoDispose(_whiteTimer.ObserveProperty(() => _whiteTimer.TotalExecutionTime)
                                   .Subscribe(_ => OnPropertyChanged(() => WhiteTime)));
        }

        public TimeSpan BlackTime
        {
            get { return _blackTimer.TotalExecutionTime; }
        }

        public TimeSpan WhiteTime
        {
            get { return _whiteTimer.TotalExecutionTime; }
        }
    }
}
