﻿using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Renju.Core
{
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

        public int X { get; set; }
        public int Y { get; set; }
        public Side Side { get; set; }
    }
}
