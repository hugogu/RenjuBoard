namespace Renju.AI
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading;
    using Infrastructure;
    using Infrastructure.AI;
    using Infrastructure.Model;

    public class AIGamePlayer : DisposableModelBase, IGamePlayer
    {
        private readonly CancellationTokenSource _aiResolvingCancelTokenSource = new CancellationTokenSource();
        private readonly IGameBoard<IReadOnlyBoardPoint> _board;
        private readonly IDropResolver _dropResolver;
        private Side _side = Side.White;

        public AIGamePlayer(IDropResolver dropResolver, IGameBoard<IReadOnlyBoardPoint> board, GameOptions options)
        {
            _board = board;
            _dropResolver = dropResolver;
            _dropResolver.CancelTaken = _aiResolvingCancelTokenSource.Token;
            AutoDispose(_aiResolvingCancelTokenSource);
            AutoDispose(options.ObserveProperty(() => options.AIFirst).Subscribe(_ =>
            {
                if (!board.DroppedPoints.Any())
                    Side = options.AIFirst ? Side.Black : Side.White;
                else
                    Trace.WriteLine("AI behavior changes will be applied to new games.");
            }));
            AutoDispose(Observable.FromEventPattern<PieceDropEventArgs>(handler => board.PieceDropped += handler, handler => board.PieceDropped -= handler)
                        .Select(e => e.EventArgs)
                        .Where(e => e.OperatorType == OperatorType.Human && board.ExpectedNextTurn == _side)
                        .TakeWhile(e => board.ExpectedNextTurn.HasValue)
                        .Subscribe(OnPieceDropped));
        }

        public Side Side
        {
            get { return _side; }
            set
            {
                _side = value;
                OnPlaygroundChanged();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _aiResolvingCancelTokenSource.Cancel();
            }

            base.Dispose(disposing);
        }

        protected virtual void OnPlaygroundChanged()
        {
            if (Side == Side.Black && _board.DropsCount == 0)
            {
                _board.Drop(new BoardPosition(7, 7), OperatorType.AI);
            }
        }

        private async void OnPieceDropped(PieceDropEventArgs args)
        {
            var drop = await _dropResolver.ResolveAsync(_board, Side);
            if (_aiResolvingCancelTokenSource.IsCancellationRequested)
                return;
            _board.Drop(drop.Position, OperatorType.AI);
        }
    }
}
