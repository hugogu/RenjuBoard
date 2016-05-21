﻿namespace Renju.Infrastructure.Model
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
            return side == Side.White ? Side.Black : Side.White;
        }
    }
}
