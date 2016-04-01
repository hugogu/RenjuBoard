using System;
using System.Collections.Generic;
using System.Linq;
using Runju.Infrastructure;

namespace Renju.Core
{
    public class BoardRecorder : ModelBase
    {
        private readonly IGameBoard _board;
        private readonly Stack<PieceDrop> _undoDrops = new Stack<PieceDrop>();
        private readonly Stack<PieceDrop> _redoDrops = new Stack<PieceDrop>();

        public BoardRecorder(IGameBoard board)
        {
            _board = board;
            _board.PieceDropped += OnBoardPieceDropped;
        }

        public IEnumerable<PieceDrop> Drops
        {
            get { return _undoDrops; }
        }

        public IEnumerable<PieceDrop> RedoDrops
        {
            get { return _redoDrops; }
        }

        public bool CanUndo
        {
            get { return _undoDrops.Any(); }
        }

        public bool CanRedo
        {
            get { return _redoDrops.Any(); }
        }

        private void OnBoardPieceDropped(object sender, PieceDropEventArgs e)
        {
            _undoDrops.Push(e.Drop);
            RaisePropertyChanged(() => CanUndo);
        }

        public void UndoDrop()
        {
            if (_undoDrops.Count == 0)
                throw new InvalidOperationException("There is no drop to undo.");

            var drop = _undoDrops.Pop();
            _board.Take(new BoardPosition(drop.X, drop.Y));
            _redoDrops.Push(drop);
            RaisePropertyChanged(() => CanUndo);
            RaisePropertyChanged(() => CanRedo);
        }

        public void RedoDrop()
        {
            if (_redoDrops.Count == 0)
                throw new InvalidOperationException("There is no drop to redo.");

            var drop = _redoDrops.Pop();
            _board.Drop(new BoardPosition(drop.X, drop.Y));
        }
    }
}
