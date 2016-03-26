using System;
using System.Linq;
using Renju.Core;

namespace Renju.AI
{
    public class AIGamePlayer : IGamePlayer
    {
        private GameBoard _board;
        private IDropSelector _dropSelector;

        public AIGamePlayer(IDropSelector dropSelector)
        {
            _dropSelector = dropSelector;
        }

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

        public Side Side { get; set; }

        private void OnBoardPieceDropped(object sender, PieceDropEventArgs e)
        {
            if (Board.ExpectedNextTurn == Side)
            {
                var drops = _dropSelector.SelectDrops(Board, Side);
                Board.Drop(drops.First());
            }
        }
    }
}
