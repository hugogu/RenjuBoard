using System;

namespace Renju.Core
{
    public class PieceDropEventArgs : EventArgs
    {
        public PieceDropEventArgs(PieceDrop drop, OperatorType operatorType)
        {
            Drop = drop;
            OperatorType = operatorType;
        }

        public PieceDrop Drop { get; internal set; }

        public OperatorType OperatorType { get; internal set; }
    }
}
