namespace RenjuBoard.ViewModels
{
    using System;
    using System.Reactive.Linq;
    using Microsoft.Practices.Unity;
    using Renju.Core;
    using Renju.Infrastructure;
    using Renju.Infrastructure.Execution;
    using Renju.Infrastructure.Model;

    public class BoardTimingViewModel : DisposableModelBase
    {
        private readonly ExecutionTimer _blackTimer;
        private readonly ExecutionTimer _whiteTimer;

        public BoardTimingViewModel(IGameBoard<IReadOnlyBoardPoint> gameBoard,
            [Dependency("Black")] IGamePlayer blackPlayer,
            [Dependency("White")] IGamePlayer whitePlayer)
        {
            _blackTimer = gameBoard.GetExecutionTimer(blackPlayer);
            _whiteTimer = gameBoard.GetExecutionTimer(whitePlayer);
            AutoDispose(_blackTimer);
            AutoDispose(_whiteTimer);
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
