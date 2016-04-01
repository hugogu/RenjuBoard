using System.ComponentModel;
using System.Diagnostics;
using Runju.Infrastructure;

namespace Renju.Core
{
    [DebuggerDisplay("[{Index}-({Position.X},{Position.Y}):{Status}]")]
    public class BoardPoint : ModelBase, IReadOnlyBoardPoint
    {
        private int? _index;
        private Side? _status = null;
        private int _weight;

        public BoardPoint(BoardPosition position)
        {
            Position = position;
        }

        [ReadOnly(true)]
        public BoardPosition Position { get; private set; }

        public int? Index
        {
            get { return _index; }
            set { SetProperty(ref _index, value, () => Index); }
        }

        public Side? Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value, () => Status); }
        }

        public int Weight
        {
            get { return _weight; }
            set { SetProperty(ref _weight, value, () => Weight); }
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", Position, Status == null ? "" : (Status.Value == Side.Black ? "●" : "○"));
        }

        internal void ResetToEmpty()
        {
            Index = null;
            Status = null;
        }
    }
}
