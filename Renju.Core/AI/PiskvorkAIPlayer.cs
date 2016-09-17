namespace Renju.Core.AI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using Infrastructure.AI;
    using Infrastructure.Events;
    using Infrastructure.Execution;
    using Infrastructure.Model;
    using Infrastructure.Protocols.Piskvork;
    using Microsoft.Practices.Unity;

    [DisplayName("Piskvork AI")]
    public class PiskvorkAIPlayer : RenjuBoardAIPlayer, IReportResourceUsage
    {
        private readonly IAIController _piskvorkAI;
        private readonly ProcessResourceUsageMonitor _processMonitor = new ProcessResourceUsageMonitor();

        [Dependency]
        public IReadBoardState<IReadOnlyBoardPoint> RenjuBoard { get; set; }

        public PiskvorkAIPlayer([Description("Execution File")] string piskvorkAIExecutionFile)
        {
            var piskvorkAI = new PiskvorkAIPlayerAdapter(piskvorkAIExecutionFile);
            piskvorkAI.Dropping += OnAIDropping;
            piskvorkAI.Says += OnAISays;
            _piskvorkAI = piskvorkAI;
            _processMonitor.Monitor(piskvorkAI.Process, TimeSpan.FromMilliseconds(1000));
            AutoDispose(_processMonitor);
            AutoCallOnDisposing(() =>
            {
                _piskvorkAI.Dropping -= OnAIDropping;
                _piskvorkAI.Says -= OnAISays;
            });
            AutoDispose(_piskvorkAI as IDisposable);
        }

        public IObservable<ResourceUsageInformation> ResourceUsages
        {
            get { return _processMonitor.UsageInfo; }
        }

        [ReadOnly(true)]
        public override string AuthorName
        {
            get { return _piskvorkAI.AIInfo.Author; }
            set { throw new NotSupportedException(); }
        }

        [ReadOnly(true)]
        public override string Country
        {
            get { return _piskvorkAI.AIInfo.Country; }
            set { throw new NotSupportedException(); }
        }

        [ReadOnly(true)]
        public override string Name
        {
            get { return _piskvorkAI.AIInfo.Name; }
            set { throw new NotSupportedException(); }
        }

        protected override async void OnInitailizing(object sender, GenericEventArgs<int> e)
        {
            await _piskvorkAI.Initialize(e.Message);
        }

        protected override async void OnAboutRequested(object sender, EventArgs e)
        {
            await _piskvorkAI.RequestAbout();
        }

        protected override async void OnBoardDropped(object sender, GenericEventArgs<BoardPosition> e)
        {
            if (RenjuBoard[e.Message].Status != Side)
                await _piskvorkAI.OpponentDrops(e.Message);
        }

        protected override void OnBoardDropTaken(object sender, GenericEventArgs<BoardPosition> e)
        {
            throw new NotSupportedException();
        }

        protected override async void OnPlayerStarting()
        {
            await _piskvorkAI.Begin();
        }

        protected override async void OnLoadingBoard(object sender, GenericEventArgs<IEnumerable<PieceDrop>> e)
        {
            await _piskvorkAI.Load(e.Message);
        }

        private void OnAISays(object sender, GenericEventArgs<string> e)
        {
            Operator.ShowMessage(e.Message, true);
        }

        private void OnAIDropping(object sender, GenericEventArgs<BoardPosition> e)
        {
            try
            {
                Operator.Put(e.Message);
            }
            catch (InvalidOperationException ex)
            {
                Trace.TraceError(ex.Message);
                _piskvorkAI.Begin();
            }
        }
    }
}
