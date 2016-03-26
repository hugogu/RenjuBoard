using System;
using System.Linq;
using Renju.Core;

namespace Renju.AI
{
    public class AIGamePlayer : IGamePlayer
    {
        private GameBoard _board;

        public GameBoard Board
        {
            get { return _board; }
            set
            {
                if (_board != null)
                    throw new InvalidOperationException();
                if (value == null)
                    throw new ArgumentNullException("value");
                _board = value;
                _board.PieceDropped += OnBoardPieceDropped;
            }
        }

        public Side Side
        {
            get;
            set;
        }

        private void OnBoardPieceDropped(object sender, PieceDropEventArgs e)
        {
            if (Board.Drops.Last().Side != Side)
            {
                var drop = Board[e.Drop.X + 1, e.Drop.Y];
                while (drop.Status != null)
                    drop = Board[drop.Position.X + 1, drop.Position.Y];

                Board.Drop(drop);
            }
        }
    }
}
