using System;
using System.Diagnostics;

namespace Renju.Core
{
    [DebuggerDisplay("(X={X}, Y={Y})")]
    public class BoardPosition : IEquatable<BoardPosition>
    {
        public BoardPosition() { }

        public BoardPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public static BoardPosition operator +(BoardPosition a, BoardPosition b)
        {
            return new BoardPosition(a.X + b.X, a.Y + b.Y);
        }

        public static BoardPosition operator -(BoardPosition a, BoardPosition b)
        {
            return new BoardPosition(a.X - b.X, a.Y - b.Y);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BoardPosition);
        }

        public bool Equals(BoardPosition other)
        {
            if (other == null)
                return false;

            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return X << 4 + Y;
        }
    }
}
