namespace Renju.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using Infrastructure;
    using Infrastructure.AI;
    using Infrastructure.Events;
    using Infrastructure.Model;
    using Infrastructure.Protocols;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Utility;

    public class AIGamePlayer : DisposableModelBase, IGamePlayer
    {
        private readonly CancellationTokenSource _aiResolvingCancelTokenSource = new CancellationTokenSource();
        private readonly IBoardOperator _operator;
        private readonly IDropResolver _resolver;

        public AIGamePlayer(IBoardMonitor monitor, IBoardOperator operater, IDropResolver resolver)
        {
            Guard.ArgumentNotNull(monitor, "monitor");
            Guard.ArgumentNotNull(operater, "operater");
            Guard.ArgumentNotNull(resolver, "resolver");

            _operator = operater;
            _resolver = resolver;

            resolver.CancelTaken = _aiResolvingCancelTokenSource.Token;

            monitor.Loading += OnLoadingBoard;
            monitor.Dropped += OnBoardDropped;
            monitor.Taken += OnBoardDropTaken;
            monitor.Starting += OnBoardStarting;
            monitor.Ended += OnGameEnded;
            monitor.AboutRequested += OnAboutRequested;

            AutoDispose(_aiResolvingCancelTokenSource);
            AutoCallOnDisposing(() =>
            {
                monitor.Loading -= OnLoadingBoard;
                monitor.Dropped -= OnBoardDropped;
                monitor.Taken -= OnBoardDropTaken;
                monitor.Starting -= OnBoardStarting;
                monitor.Ended -= OnGameEnded;
                monitor.AboutRequested -= OnAboutRequested;
            });
        }

        [Dependency("AI")]
        public IGameBoard<IReadOnlyBoardPoint> VirtualAIGameBoard { get; set; }

        public Side Side { get; set; } = Side.White;

        public string Name { get; set; }

        public string AuthorName { get; set; }

        public string Country { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _aiResolvingCancelTokenSource.Cancel();
                _aiResolvingCancelTokenSource.Dispose();
            }

            base.Dispose(disposing);
        }

        protected virtual void OnBoardStarting(object sender, EventArgs e)
        {
            Debug.Assert(VirtualAIGameBoard.DropsCount == 0, "Starting event is not valid when game already started.");
            Side = Side.Black;
            VirtualAIGameBoard.Drop(new BoardPosition(7, 7), OperatorType.AI);
        }

        protected virtual void OnGameEnded(object sender, EventArgs e)
        {
        }

        protected virtual void OnLoadingBoard(object sender, GenericEventArgs<IEnumerable<PieceDrop>> e)
        {
            foreach (var drop in e.Message)
                VirtualAIGameBoard.Drop(drop, OperatorType.Loading);
            OnPieceDropped();
        }

        protected virtual void OnBoardDropped(object sender, GenericEventArgs<BoardPosition> e)
        {
            VirtualAIGameBoard.Drop(e.Message, OperatorType.Human);
            if (VirtualAIGameBoard[e.Message].Status == Sides.Opposite(Side))
            {
                OnPieceDropped();
            }
        }

        protected virtual void OnBoardDropTaken(object sender, GenericEventArgs<BoardPosition> e)
        {
            VirtualAIGameBoard.Take(e.Message);
        }

        private async void OnPieceDropped()
        {
            var drop = await _resolver.ResolveAsync(VirtualAIGameBoard, Side);
            if (_aiResolvingCancelTokenSource.IsCancellationRequested)
                return;
            _operator.Put(new PieceDrop(drop.Position, Side));
        }

        private void OnAboutRequested(object sender, EventArgs e)
        {
            var info = new AIInfo()
            {
                Name = Name,
                Author = AuthorName,
                Country = Country,
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };
            _operator.ShowInfo(info);
        }
    }
}
