﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Renju.Infrastructure;
using Renju.Infrastructure.Model;

namespace Renju.AI
{
    public class AIGamePlayer : DisposableModelBase, IGamePlayer
    {
        private Task _runningTask;
        private CancellationTokenSource _aiResolvingCancelTokenSource = new CancellationTokenSource();
        private Side _side = Side.White;
        private IGameBoard<IReadOnlyBoardPoint> _board;
        private readonly IDropResolver _dropResolver;

        public AIGamePlayer(IDropResolver dropResolver, GameOptions options)
        {
            _dropResolver = dropResolver;
            _dropResolver.CancelTaken = _aiResolvingCancelTokenSource.Token;
            AutoDispose(_aiResolvingCancelTokenSource);
            AutoDispose(options.ObserveProperty(() => options.AIFirst).Subscribe(_ =>
            {
                if (!Board.DroppedPoints.Any())
                    Side = options.AIFirst ? Side.Black : Side.White;
                else
                    Trace.WriteLine("AI behavior changes will be applied to new games.");
            }));
        }

        [Dependency]
        public IGameBoard<IReadOnlyBoardPoint> Board
        {
            get { return _board; }
            set
            {
                if (_board != null)
                    throw new InvalidOperationException();
                if (value == null)
                    throw new ArgumentNullException("value");
                _board = value;
                _board.PieceDropped += OnBoardPieceDropped;
                OnPlaygroundChanged();
            }
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
                if (_runningTask != null)
                {
                    _aiResolvingCancelTokenSource.Cancel();
                    _runningTask.Wait();
                }
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

        private void OnBoardPieceDropped(object sender, PieceDropEventArgs e)
        {
            if (Board.ExpectedNextTurn == Side && e.OperatorType == OperatorType.Human)
            {
                _runningTask = Task.Factory.StartNew(() =>
                {
                    var drops = _dropResolver.Resolve(Board, Side).First();
                    if (_aiResolvingCancelTokenSource.IsCancellationRequested)
                        return;
                    Board.Drop(drops.Position, OperatorType.AI);
                    _runningTask = null;
                }, _aiResolvingCancelTokenSource.Token);
                AutoDispose(_runningTask);
            }
        }
    }
}
