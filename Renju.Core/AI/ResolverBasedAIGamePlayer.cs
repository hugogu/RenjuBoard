namespace Renju.Core.AI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using Infrastructure.AI;
    using Infrastructure.Events;
    using Infrastructure.Model;
    using Infrastructure.Protocols;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Utility;

    public class ResolverBasedAIGamePlayer : RenjuBoardAIPlayer
    {
        private readonly CancellationTokenSource _aiResolvingCancelTokenSource = new CancellationTokenSource();
        private readonly IDropResolver _resolver;

        public ResolverBasedAIGamePlayer(IDropResolver resolver)
        {
            Guard.ArgumentNotNull(resolver, "resolver");

            _resolver = resolver;

            resolver.CancelTaken = _aiResolvingCancelTokenSource.Token;
            AutoDispose(_aiResolvingCancelTokenSource);
        }

        [Dependency("AI")]
        public IGameBoard<IReadOnlyBoardPoint> VirtualAIGameBoard { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _aiResolvingCancelTokenSource.Cancel();
                _aiResolvingCancelTokenSource.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnInitailizing(object sender, GenericEventArgs<int> e)
        {
            Trace.TraceInformation("Initializing board size " + e.Message);
        }

        protected override void OnBoardStarting(object sender, EventArgs e)
        {
            Debug.Assert(VirtualAIGameBoard.DropsCount == 0, "Starting event is not valid when game already started.");
            Side = Side.Black;
            VirtualAIGameBoard.Drop(new BoardPosition(7, 7), OperatorType.AI);
        }

        protected override void OnLoadingBoard(object sender, GenericEventArgs<IEnumerable<PieceDrop>> e)
        {
            foreach (var drop in e.Message)
                VirtualAIGameBoard.Drop(drop, OperatorType.Loading);
            OnPieceDropped();
        }

        protected override void OnBoardDropped(object sender, GenericEventArgs<BoardPosition> e)
        {
            VirtualAIGameBoard.Drop(e.Message, OperatorType.Human);
            if (VirtualAIGameBoard[e.Message].Status == Sides.Opposite(Side))
            {
                OnPieceDropped();
            }
        }

        protected override void OnBoardDropTaken(object sender, GenericEventArgs<BoardPosition> e)
        {
            VirtualAIGameBoard.Take(e.Message);
        }

        private async void OnPieceDropped()
        {
            var drop = await _resolver.ResolveAsync(VirtualAIGameBoard, Side);
            if (_aiResolvingCancelTokenSource.IsCancellationRequested)
                return;
            Operator.Put(new PieceDrop(drop.Position, Side));
        }

        protected override void OnAboutRequested(object sender, EventArgs e)
        {
            var info = new AIInfo()
            {
                Name = Name,
                Author = AuthorName,
                Country = Country,
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };
            Operator.ShowInfo(info);
        }
    }
}
