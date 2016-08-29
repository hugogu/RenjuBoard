namespace Renju.Core.AI
{
    using System;
    using System.Collections.Generic;
    using Infrastructure.Events;
    using Infrastructure.Model;
    using Infrastructure.Protocols;
    using Infrastructure.Protocols.Piskvork;

    public class PiskvorkAIPlayer : RenjuBoardAIPlayer, IGamePlayer
    {
        private PiskvorkAIPlayerAdapter _piskvorkAI;

        public PiskvorkAIPlayer(string piskvorkAIExecutionFile, IBoardOperator boardOperator)
            : base(boardOperator)
        {
            _piskvorkAI = new PiskvorkAIPlayerAdapter(piskvorkAIExecutionFile);
            _piskvorkAI.Dropping += OnAIDropping;
            _piskvorkAI.Says += OnAISays;
            AutoCallOnDisposing(() =>
            {
                _piskvorkAI.Dropping -= OnAIDropping;
                _piskvorkAI.Says -= OnAISays;
            });
            AutoDispose(_piskvorkAI);
        }

        public override string AuthorName
        {
            get { return _piskvorkAI.AIInfo.Author; }
            set { throw new NotSupportedException(); }
        }

        public override string Country
        {
            get { return _piskvorkAI.AIInfo.Country; }
            set { throw new NotSupportedException(); }
        }

        public override string Name
        {
            get { return _piskvorkAI.AIInfo.Name; }
            set { throw new NotSupportedException(); }
        }

        protected override void OnInitailizing(object sender, GenericEventArgs<int> e)
        {
            _piskvorkAI.Initialize(e.Message);
        }

        protected override void OnAboutRequested(object sender, EventArgs e)
        {
            _piskvorkAI.RequestAbout();
        }

        protected override void OnBoardDropped(object sender, GenericEventArgs<BoardPosition> e)
        {
            _piskvorkAI.OpponentDrops(e.Message);
        }

        protected override void OnBoardDropTaken(object sender, GenericEventArgs<BoardPosition> e)
        {
            throw new NotSupportedException();
        }

        protected override void OnBoardStarting(object sender, EventArgs e)
        {
            _piskvorkAI.Begin();
        }

        protected override void OnLoadingBoard(object sender, GenericEventArgs<IEnumerable<PieceDrop>> e)
        {
            _piskvorkAI.Load(e.Message);
        }

        private void OnAISays(object sender, GenericEventArgs<string> e)
        {
            Operator.ShowMessage(e.Message, true);
        }

        private void OnAIDropping(object sender, GenericEventArgs<BoardPosition> e)
        {
            Operator.Put(e.Message);
        }
    }
}
