namespace Renju.Infrastructure.Model
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    [Serializable]
    [DebuggerDisplay("{Side}: X={X}, Y={Y}")]
    [TypeConverter(typeof(PieceDropTypeConverter))]
    public class PieceDrop
    {
        public PieceDrop(int x, int y, Side side)
        {
            X = x;
            Y = y;
            Side = side;
        }

        public PieceDrop(BoardPosition position, Side side)
            : this(position.X, position.Y, side)
        {
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        public Side Side { get; private set; }

        public override string ToString()
        {
            return String.Format("({0},{1}){2}", X, Y, Side == Side.Black ? "●" : "○");
        }
    }
}
