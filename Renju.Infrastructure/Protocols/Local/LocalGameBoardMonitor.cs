namespace Renju.Infrastructure.Protocols.Local
{
    using System;
    using System.Collections.Generic;
    using Events;
    using Model;

    public class LocalGameBoardMonitor : DisposableModelBase, IBoardMonitor
    {
        private readonly IGameBoard<IReadOnlyBoardPoint> _gameBoard;

        public LocalGameBoardMonitor(IGameBoard<IReadOnlyBoardPoint> gameBoard, GameOptions options)
        {
            _gameBoard = gameBoard;
            AutoDispose(options.ObserveProperty(() => options.AIFirst).Subscribe(_ => RaiseEvent(Starting)));
            gameBoard.Begin += OnGameBegin;
            gameBoard.PieceDropped += OnPieceDropped;
            gameBoard.Taken += OnPieceTaken;

            AutoCallOnDisposing(() =>
            {
                gameBoard.Begin -= OnGameBegin;
                gameBoard.Taken -= OnPieceTaken;
                gameBoard.PieceDropped -= OnPieceDropped;
            });
        }

        public event EventHandler<GenericEventArgs<BoardPosition>> Dropped;

        public event EventHandler Ended;

        public event EventHandler<GenericEventArgs<int>> Initailizing;

        public event EventHandler<GenericEventArgs<IEnumerable<PieceDrop>>> Loading;

        public event EventHandler Starting;

        public event EventHandler<GenericEventArgs<BoardPosition>> Taken;

        public event EventHandler AboutRequested;

        protected override void Dispose(bool disposing)
        {
            if (disposing && _gameBoard.ExpectedNextTurn.HasValue)
            {
                RaiseEvent(Ended);
            }

            base.Dispose(disposing);
        }

        private void OnGameBegin(object sender, GenericEventArgs<NewGameOptions> e)
        {
            RaiseEvent(Initailizing, new GenericEventArgs<int>(e.Message.BoardSize));
        }

        private void OnPieceTaken(object sender, BoardPosition e)
        {
            RaiseEvent(Taken, new GenericEventArgs<BoardPosition>(e));
        }

        private void OnPieceDropped(object sender, PieceDropEventArgs e)
        {
            if (e.OperatorType == OperatorType.Loading)
            {
                RaiseEvent(Loading, new GenericEventArgs<IEnumerable<PieceDrop>>(new[] { e.Drop }));
            }

            RaiseEvent(Dropped, new GenericEventArgs<BoardPosition>(e.Drop));

            if (_gameBoard.ExpectedNextTurn == null)
            {
                RaiseEvent(Ended);
            }
        }
    }
}
