using System;

namespace Renju.Core
{
    public class PieceDropEventArgs : EventArgs
    {
        public PieceDropEventArgs(PieceDrop drop)
        {
            Drop = drop;
        }

        public PieceDrop Drop { get; set; }
    }
}
