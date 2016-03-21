using System.ComponentModel;
using System.Diagnostics;
using Runju.Infrastructure;

namespace Renju.Core
{
    [DebuggerDisplay("[{Index}-{Position}:{Status}]")]
    public class BoardPoint : ModelBase
    {
        private int? _index;
        private Side? _status = null;

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
    }
}
