namespace Renju.Infrastructure.Model
{
    using System;

    [Serializable]
    public enum Side : int
    {
        Black = 1,
        White = 2,
    }

    public static class Sides
    {
        public static Side Opposite(Side side)
        {
            if (side == Side.Black)
                return Side.White;
            if (side == Side.White)
                return Side.Black;

            throw new ArgumentOutOfRangeException("side");
        }
    }
}
