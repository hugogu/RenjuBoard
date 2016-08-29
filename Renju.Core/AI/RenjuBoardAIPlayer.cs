namespace Renju.Core.AI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Infrastructure;
    using Infrastructure.Events;
    using Infrastructure.Model;
    using Infrastructure.Protocols;
    using Microsoft.Practices.Unity.Utility;

    public abstract class RenjuBoardAIPlayer : DisposableModelBase, IGamePlayer
    {
        public virtual string AuthorName { get; set; }
        public virtual string Country { get; set; }
        public virtual string Name { get; set; }
        public virtual Side Side { get; set; } = Side.White;

        public RenjuBoardAIPlayer(IBoardOperator boardOperator)
        {
            Guard.ArgumentNotNull(boardOperator, "boardOperator");

            Operator = boardOperator;
        }

        public virtual void PlayOn(IBoardMonitor monitor)
        {
            Guard.ArgumentNotNull(monitor, "monitor");

            monitor.Initailizing += OnInitailizing;
            monitor.Loading += OnLoadingBoard;
            monitor.Dropped += OnBoardDropped;
            monitor.Taken += OnBoardDropTaken;
            monitor.Starting += OnBoardStarting;
            monitor.Ended += OnGameEnded;
            monitor.AboutRequested += OnAboutRequested;

            AutoCallOnDisposing(() =>
            {
                monitor.Initailizing -= OnInitailizing;
                monitor.Loading -= OnLoadingBoard;
                monitor.Dropped -= OnBoardDropped;
                monitor.Taken -= OnBoardDropTaken;
                monitor.Starting -= OnBoardStarting;
                monitor.Ended -= OnGameEnded;
                monitor.AboutRequested -= OnAboutRequested;
            });
        }

        protected IBoardOperator Operator { get; private set; }

        protected abstract void OnInitailizing(object sender, GenericEventArgs<int> e);

        protected abstract void OnBoardStarting(object sender, EventArgs e);

        protected virtual void OnGameEnded(object sender, EventArgs e)
        {
            Trace.TraceInformation("Game ends.");
        }

        protected abstract void OnLoadingBoard(object sender, GenericEventArgs<IEnumerable<PieceDrop>> e);

        protected abstract void OnBoardDropped(object sender, GenericEventArgs<BoardPosition> e);

        protected abstract void OnBoardDropTaken(object sender, GenericEventArgs<BoardPosition> e);

        protected virtual void OnAboutRequested(object sender, EventArgs e)
        {
        }
    }
}
