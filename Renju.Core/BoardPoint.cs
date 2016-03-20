using System.ComponentModel;
using System.Diagnostics;
using Runju.Infrastructure;

namespace Renju.Core
{
    [DebuggerDisplay("[{Position}:{Status}]")]
    public class BoardPoint : ModelBase
    {
        public BoardPoint(BoardPosition position)
        {
            Position = position;
        }

        [ReadOnly(true)]
        public BoardPosition Position { get; private set; }

        private Side? _status = null;
        public Side? Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value, () => Status); }
        }
    }
}
