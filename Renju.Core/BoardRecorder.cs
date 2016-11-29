namespace Renju.Core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Infrastructure;
    using Infrastructure.Model;
    using Prism.Commands;

    public class BoardRecorder : ModelBase
    {
        private readonly IGameBoard<IReadOnlyBoardPoint> _board;
        private readonly List<PieceDrop> _undoDrops = new List<PieceDrop>();
        private readonly List<PieceDrop> _redoDrops = new List<PieceDrop>();

        public BoardRecorder(IGameBoard<IReadOnlyBoardPoint> board)
        {
            _board = board;
            _board.PieceDropped += OnBoardPieceDropped;
            this.PropertyChanged += OnBoardRecorderPropertyChanged;
            UndoCommand = new DelegateCommand(() => UndoDrop(), () => CanUndo);
            RedoCommand = new DelegateCommand(() => RedoDrop(), () => CanRedo);
        }

        public DelegateCommand UndoCommand { get; private set; }

        public DelegateCommand RedoCommand { get; private set; }

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
            get { return _undoDrops.Count > 0; }
        }

        public bool CanRedo
        {
            get { return _redoDrops.Count > 0; }
        }

        public void ClearGameBoard()
        {
            while (CanUndo)
                UndoDrop();
        }

        public void UndoDrop()
        {
            Contract.Requires(CanUndo, "There is no drop to undo.");

            var drop = _undoDrops.Last();
            _undoDrops.RemoveAt(_undoDrops.Count - 1);
            _board.Take(drop);
            _redoDrops.Add(drop);
            OnPropertyChanged(() => CanUndo);
            OnPropertyChanged(() => CanRedo);
        }

        public void RedoDrop()
        {
            Contract.Requires(CanRedo, "There is no drop to redo.");

            var drop = _redoDrops.Last();
            _redoDrops.RemoveAt(_redoDrops.Count - 1);
            _board.Drop(drop, OperatorType.UndoOrRedo);
        }

        private void OnBoardPieceDropped(object sender, PieceDropEventArgs e)
        {
            _undoDrops.Add(e.Drop);
            OnPropertyChanged(() => CanUndo);
        }

        private void OnBoardRecorderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RunInDispatcher(() =>
            {
                UndoCommand.RaiseCanExecuteChanged();
                RedoCommand.RaiseCanExecuteChanged();
            });
        }
    }
}
