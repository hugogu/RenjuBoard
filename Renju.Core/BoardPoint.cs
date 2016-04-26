using System;
using System.ComponentModel;
using System.Diagnostics;
using Renju.Infrastructure;
using Renju.Infrastructure.Model;

namespace Renju.Core
{
    [DebuggerDisplay("[{Index}-({Position.X},{Position.Y}):{Status}]")]
    public class BoardPoint : ModelBase, IReadOnlyBoardPoint
    {
        private int? _index;
        private Side? _status = null;
        private int _weight;
        private bool _requireReevaluateWeight = true;

        public BoardPoint(BoardPosition position)
        {
            Position = position;
        }

        public static Func<int, BoardPoint> CreateIndexBasedFactory(int size)
        {
            return index => new BoardPoint(new BoardPosition(index % size, index / size));
        }

        [ReadOnly(true)]
        public BoardPosition Position { get; private set; }

        public int? Index
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }

        public Side? Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        public int Weight
        {
            get { return _weight; }
            set { SetProperty(ref _weight, value); }
        }

        public bool RequiresReevaluateWeight
        {
            get { return _requireReevaluateWeight; }
            set { SetProperty(ref _requireReevaluateWeight, value); }
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", Position, Status == null ? "" : (Status.Value == Side.Black ? "●" : "○"));
        }

        public void ResetToEmpty()
        {
            Index = null;
            Status = null;
        }

        protected override void OnConstructingNewObject() { /* Noop */ }
    }
}
