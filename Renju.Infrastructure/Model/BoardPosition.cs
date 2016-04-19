using System;
using System.Diagnostics;

namespace Renju.Infrastructure.Model
{
    [DebuggerDisplay("[X={X}, Y={Y}]")]
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

        public static BoardPosition operator -(BoardPosition position)
        {
            return new BoardPosition(-position.X, -position.Y);
        }

        public static BoardPosition operator *(BoardPosition position, int length)
        {
            return new BoardPosition(position.X * length, position.Y * length);
        }

        public static implicit operator BoardPosition(PieceDrop drop)
        {
            return new BoardPosition(drop.X, drop.Y);
        }

        public override string ToString()
        {
            return String.Format("({0},{1})", X, Y);
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
