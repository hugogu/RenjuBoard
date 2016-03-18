using System;

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
            return false;
        }
    }
}
