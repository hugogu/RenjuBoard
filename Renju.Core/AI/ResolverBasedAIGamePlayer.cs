namespace Renju.Core.AI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Infrastructure;
    using Infrastructure.AI;
    using Infrastructure.Events;
    using Infrastructure.Model;
    using Infrastructure.Protocols;
    using Microsoft.Practices.Unity;

    [DisplayName("Renju Built-in AI")]
    public class ResolverBasedAIGamePlayer : RenjuBoardAIPlayer
    {
        private readonly CancellationTokenSource _aiResolvingCancelTokenSource = new CancellationTokenSource();

        public ResolverBasedAIGamePlayer([Description("AI")] IDropResolver resolver)
        {
            Debug.Assert(resolver != null);

            Resolver = resolver;

            resolver.CancelTaken = _aiResolvingCancelTokenSource.Token;
            AutoDispose(_aiResolvingCancelTokenSource);
        }

        [Dependency("AI")]
        public IGameBoard<IReadOnlyBoardPoint> VirtualAIGameBoard { get; set; }

        public IDropResolver Resolver { get; private set; }

        [ReadOnly(true)]
        public override string Name
        {
            get { return Resolver.GetType().Assembly.GetTitle(); }
            set { throw new NotSupportedException(); }
        }

        [ReadOnly(true)]
        public override string AuthorName
        {
            get { return Resolver.GetType().Assembly.GetCompany(); }
            set { throw new NotSupportedException(); }
        }

        [ReadOnly(true)]
        public override string Country
        {
            get { return Resolver.GetType().Assembly.GetCultureInfo().NativeName; }
            set { throw new NotSupportedException(); }
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

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

        protected override void OnPlayerStarting()
        {
            Debug.Assert(!VirtualAIGameBoard.DroppedPoints.Any(), "Starting event is not valid when game already started.");
            Debug.Assert(Side == Side.Black);
            Operator.Put(new PieceDrop(new BoardPosition(7, 7), Side));
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
            var drop = await Resolver.ResolveAsync(VirtualAIGameBoard, Side).ConfigureAwait(true);
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
