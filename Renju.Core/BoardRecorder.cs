﻿using System;
using System.Collections.Generic;
using System.Linq;
using Renju.Infrastructure;
using Renju.Infrastructure.Model;

namespace Renju.Core
{
    public class BoardRecorder : ModelBase
    {
        private readonly IGameBoard<IReadOnlyBoardPoint> _board;
        private readonly List<PieceDrop> _undoDrops = new List<PieceDrop>();
        private readonly List<PieceDrop> _redoDrops = new List<PieceDrop>();

        public BoardRecorder(IGameBoard<IReadOnlyBoardPoint> board)
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
            _undoDrops.Add(e.Drop);
            OnPropertyChanged(() => CanUndo);
        }

        public void UndoDrop()
        {
            if (_undoDrops.Count == 0)
                throw new InvalidOperationException("There is no drop to undo.");

            var drop = _undoDrops.Last();
            _undoDrops.RemoveAt(_undoDrops.Count - 1);
            _board.Take(drop);
            _redoDrops.Add(drop);
            OnPropertyChanged(() => CanUndo);
            OnPropertyChanged(() => CanRedo);
        }

        public void RedoDrop()
        {
            if (_redoDrops.Count == 0)
                throw new InvalidOperationException("There is no drop to redo.");

            var drop = _redoDrops.Last();
            _redoDrops.RemoveAt(_redoDrops.Count - 1);
            _board.Drop(drop, OperatorType.UndoOrRedo);
        }
    }
}
