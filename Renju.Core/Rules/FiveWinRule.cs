using System;
using System.Linq;
using Renju.Core;

namespace Renju.Core.Rules
{
    public class FiveWinRule : WinRule
    {
        public override string Name
        {
            get { return "Win"; }
        }

        public override bool? Win(GameBoard board, PieceDrop drop)
        {
            return board[drop.X, drop.Y].GetLinesOnBoard(board).Any(line => line.Length > 4);
        }
    }
}
